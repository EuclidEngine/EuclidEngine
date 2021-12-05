#include "UnityCG.cginc"
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members worldPos)
#pragma exclude_renderers d3d11
#define PI 3.14159265359

struct v2g {
    float4 worldPos : TEXCOORD0;
    float4 normal : NORMAL;
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
};
struct g2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
};
struct sphericalPos {
    float r;
    float3 angles;
};

float4 _Origin;
float _Radius;
float _Height;
int _Activate;

float4 sphericalToCartesian(sphericalPos pos)
{
    float4 newpos;
    newpos[0] = pos.r * cos(pos.angles[0]);
    newpos[1] = pos.r * sin(pos.angles[0]) * cos(pos.angles[1]);
    newpos[2] = pos.r * sin(pos.angles[0]) * sin(pos.angles[1]) * cos(pos.angles[2]);
    newpos[3] = pos.r * sin(pos.angles[0]) * sin(pos.angles[1]) * sin(pos.angles[2]);
    return newpos;
}

sphericalPos cartesianToSpherical(float4 pos)
{
    sphericalPos newpos;
    newpos.r = 1;//length(pos);
    newpos.angles[0] = acos(pos.x / length(pos.wzyx));
    newpos.angles[1] = acos(pos.y / length(pos.wzy));
    if (pos.w >= 0)
        newpos.angles[2] = acos(pos.z / length(pos.wz));
    else
        newpos.angles[2] = 2 * PI - acos(pos.z / length(pos.wz));
    return newpos;
}

float4 worldToSpherical(float4 pos)
{
    //pos.w = sqrt(1 - pos.x * pos.x - pos.y * pos.y - pos.z * pos.z);
    sphericalPos spos = cartesianToSpherical(pos);
    // angles
    // angles[0] + pi/2
    // angles[1] + pi/2
    // angles[2] + pi
    // angles[2] + pi  angles[0] + pi/2
    // angles[2] + pi  angles[1] + pi/2
    spos.angles[0] = fmod(spos.angles[0], PI);
    return sphericalToCartesian(spos);
}

float4 stereographicalProjection(float4 pos)
{
    if (pos.w == 1) {
        return float4(0,0,0,0);
    } else {
        return float4(pos.xyz / (1 - pos.w), 1);
    }
}

v2g vertex(float4 vertex : POSITION, float4 normal : NORMAL)
{
    float4 worldPos = float4(mul(unity_ObjectToWorld, vertex).xyz / _Radius,1);
    worldPos = worldToSpherical(worldPos);
    worldPos = stereographicalProjection(worldPos.xwzy);
    //if (length(worldPos.xyz) <= 1) {
    //    worldPos.z = sqrt(1 - worldPos.x * worldPos.x - worldPos.y * worldPos.y);
    //    //worldPos = float3(worldPos.xy / (1 - worldPos.z), 1 / _Radius);
    //}
    //float4 worldPos = mul(unity_ObjectToWorld, vertex) / _Radius;
    //if (length(worldPos.xyz) <= 1) {
    //    worldPos.w = -sqrt(1 - worldPos.x * worldPos.x - worldPos.y * worldPos.y - worldPos.z * worldPos.z);
    //    //worldPos.y = 1 / worldPos.y;
    //    worldPos = float4(worldPos.xyz / (1 - worldPos.w), 1 / _Radius);
    //    //worldPos.y = 1 / worldPos.y;
    //}
    v2g o;
    o.worldPos = worldPos;
    o.normal = normal;
    if (_Activate)
        o.pos = mul(UNITY_MATRIX_VP, float4(worldPos.xyz * _Radius, 1));
    else
        o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, vertex));
    o.color = normalize(vertex) * 0.8 + abs(normal) * 0.2;//normalize(sphericalPos) / 2 + 0.5;
    return o;
}

g2f lerpV2f(g2f i1, g2f i2, float v)
{
    g2f o;
    o.pos = i1.pos + v * (i2.pos - i1.pos);
    o.color = i1.color + v * (i2.color - i1.color);
    return o;
}

/*
    |    /      |/|/|/  |
    |  /    =>  |/|/    | 1 Triangles => 3 Squares + 3 Triangles
    |/          |/      |                7 Points + 5 Points + 3 Points

    |            /  
    |       =>  |   
    |            \  
    _ _ _         _
            =>  /   \ 
*/
//[maxvertexcount(21)] // Maximum size of OutputStream
[maxvertexcount(15)] // Maximum size of OutputStream
void geometry(triangle v2g input[3], inout TriangleStream<g2f> OutputStream)
{
    float3 v0 = normalize(input[1].worldPos-input[0].worldPos);
    float3 v1 = normalize(input[2].worldPos-input[1].worldPos);
    float3 v2 = normalize(input[0].worldPos-input[2].worldPos);
    float3 dots = {abs(dot(v0, -v2)), abs(dot(v1, -v0)), abs(dot(v2, -v1))};
    uint rightAngle = dots[0] < dots[1] ? (dots[0] < dots[2] ? 0 : 2) : (dots[1] < dots[2] ? 1 : 2);

    g2f v00 = {input[(rightAngle + 0) % 3].pos, input[(rightAngle + 0) % 3].color};
    g2f v30 = {input[(rightAngle + 1) % 3].pos, input[(rightAngle + 1) % 3].color};
    g2f v03 = {input[(rightAngle + 2) % 3].pos, input[(rightAngle + 2) % 3].color};
    g2f v10 = lerpV2f(v00, v30, 1.f/3.f);
    g2f v20 = lerpV2f(v00, v30, 2.f/3.f);
    g2f v01 = lerpV2f(v00, v03, 1.f/3.f);
    g2f v02 = lerpV2f(v00, v03, 2.f/3.f);
    g2f v12 = lerpV2f(v03, v30, 1.f/3.f);
    g2f v21 = lerpV2f(v03, v30, 2.f/3.f);
    g2f v11 = lerpV2f(v02, v20, 1.f/2.f);
    v11.pos += input[0].normal * 0.1;
    v21.pos += input[1].normal * 0.1;
    v12.pos += input[2].normal * 0.1;

    // |/|/|/
    OutputStream.Append(v00);
    OutputStream.Append(v01);
    OutputStream.Append(v10);
    OutputStream.Append(v11);
    OutputStream.Append(v20);
    OutputStream.Append(v21);
    OutputStream.Append(v30);
    OutputStream.RestartStrip();
    // |/|/
    OutputStream.Append(v01);
    OutputStream.Append(v02);
    OutputStream.Append(v11);
    OutputStream.Append(v12);
    OutputStream.Append(v21);
    OutputStream.RestartStrip();
    // |/
    OutputStream.Append(v02);
    OutputStream.Append(v03);
    OutputStream.Append(v12);
    OutputStream.RestartStrip();
}

fixed4 fragment(g2f i) : SV_Target
{
    return i.color;
}


#if false
    // See https://www.shadertoy.com/view/wtXBRH
    // S^3 is defined as all the points in R^4 with unit length.

    // for two points 'x,y' in 'S^3' their distance is the angle between their respective vectors:
    // d(x,y) := arccos( dot(x,y) )

    // a ray 'p(t)' starting at 'o' going towards to 'd', where (o,d) = 0, is:
    // p(t) = cos(t)*o + sin(t)*d,

    //some shading parameters
    //const float FOG_DISTANCE = PI*2.;
    //const vec3  FOG_COLOR    = vec3(0.5,0.6,0.7);
    //const vec3  LIGHT_COLOR  = vec3(1.,.7,.1);
    //const vec4  LIGHT_VECTOR = normalize(vec4(1,1,1,1));

    // their are six ''axis''
    // red, green, blue, yellow, cyan, magenta
    const vec3 axisColors[6] = vec3[6](
        vec3(1,0,0),
        vec3(0,1,0),
        vec3(0,0,1),
        vec3(1,1,0),
        vec3(0,1,1),
        vec3(1,0,1)
    );

    // their are eight ''poles''
    // red, green, blue, yellow, dark red, dark green, dark blue, dark yellow
    // the dark variant is the antipodal point of the normal variant
    const vec3 poleColors[8] = vec3[8](
        vec3(1,0,0),
        vec3(0,1,0),
        vec3(0,0,1),
        vec3(1,1,0),
        vec3(.5,0,0),
        vec3(0,.5,0),
        vec3(0,0,.5),
        vec3(.5,.5,0)
    );


    // solves a*cos(x) + b*sin(x) = c
    // define
    // R^2 = a^2 + b^2
    // A := a/R, B := b/R, C := c/R
    // rewrite
    // A*cos(x) + B*sin(x) = C
    // now (A,B) is on the unit circle so there exists an angle alpha s.t.
    // sin(alpha) = A, cos(alpha) = B
    // so alpha = atan(A, B)
    // filling in
    // sin(alpha)*cos(x) + cos(alpha)*sin(x) = C
    // applying sum identity
    // sin(alpha+x) = C
    // we see that it is unsolvable if |C| > 1, or in other words if c^2 > a^2 + b^2
    // otherwise
    // x = asin(C) - alpha AND x = pi - asin(C) - alpha
    float solveThing(float a, float b, float c){
        float R = sqrt(a*a + b*b);
        float alpha = atan(a, b);
        float x = asin(c/R) - alpha;
        x = mod(x, 2.*PI);
        return x;
    }

    // stores information of a hit
    struct hit{
        float t; //hit t
        vec4  p; //hit pos
        vec4  n; //hit "normal" for the shading
    };

    // ball intersections can be done analytically
    // this function solves the system
    // d(p(t), b) = r
    // where 'b' is the position of the ball and 'r' its radius,
    // this all reduces to solving:
    // cos(t)dot(o,b) + sin(t)dot(d,b) = cos(r)
    // which can be done analytically as seen above
    hit ball(vec4 o, vec4 d, vec4 b, float r){

        float t = solveThing(dot(o,b), dot(d,b), cos(r));

        //hit pos
        vec4 p = o*cos(t)+d*sin(t);

        return hit(t, p, normalize(p-b));
    }



    // let a,b in S^3
    // L = { x in S^3 such that x is the span of a and b}
    // d(p(t), L) = r
    // the above equation expands to
    // cos^2(t)dot(ot,ot) + 2*cos(t)*sin(t)*dot(ot,dt) + sin^2(t)*dot(dt,dt) = cos^2(r)
    // where ot = (dot(o, a), dot(o, b))
    // where dt = (dot(d, a), dot(d, b))
    // luckily these are all double frequency formulations so can be reduced to an equation
    // that is like solveThing
    hit axis(vec4 _o, vec4 _d, vec4 a, vec4 b, float r){
        //reduce to 2d
        vec2 o = vec2(dot(_o,a),dot(_o,b));
        vec2 d = vec2(dot(_d,a),dot(_d,b));
        float cr = cos(r);
        float cr2 = cr*cr;

        //some variables
        float A = dot(o, o);
        float B = dot(o, d);
        float C = dot(d, d);
        float E = cr2;

        //solve, the returned thing is 2 times t
        float t = solveThing(A-C, 2.*B, 2.*E - A - C);
        //so half it
        t *= .5;

        //hitpos
        vec4 p = _o*cos(t)+_d*sin(t);

        //closest pos on the axis
        vec4 k = dot(p,a)*a + dot(p,b)*b;
        k = normalize(k);

        return hit(t, p, normalize(p-k));
    }


    hit scene(vec4 o, vec4 d, inout vec3 col){
        // initialize hit
        hit h = hit(1e20, vec4(0), vec4(0));

        // balls
        for(int i=0; i<4; i++){
            vec4 b = vec4(0);
            b[i] = 1.;
            hit nh = ball(o, d, b, 0.1);
            if(nh.t<h.t){
                h = nh;
                col = poleColors[i];
            }
        }

        // antipodal balls
        for(int i=0; i<4; i++){
            vec4 b = vec4(0);
            b[i] = -1.;
            hit nh = ball(o, d, b, 0.1);
            if(nh.t<h.t){
                h = nh;
                col = poleColors[i+4];
            }
        }

        // "axis"
        int c = 0;
        for(int i=0; i<4; i++){
            for(int j=i+1; j<4; j++){
                vec4 a = vec4(0);
                vec4 b = vec4(0);
                a[i] = 1.;
                b[j] = 1.;
                hit nh = axis(o, d, a, b, 0.05);
                if(nh.t<h.t){
                    h = nh;
                    col = axisColors[c];
                }
                c++;
            }
        }

        return h;
    }

    void mainImage( out vec4 fragColor, in vec2 fragCoord )
    {
        vec2 p = (fragCoord*2.-iResolution.xy)/iResolution.y*1.;

        // get camera
        vec4 cameraPosition = texelFetch(iChannel0,ivec2(0,0),0);
        vec4 cameraForward  = texelFetch(iChannel0,ivec2(1,0),0);
        vec4 cameraRight    = texelFetch(iChannel0,ivec2(2,0),0);
        vec4 cameraUpward   = texelFetch(iChannel0,ivec2(3,0),0);

        // create ray
        vec4 ori = cameraPosition;
        vec4 dir = normalize(cameraForward+p.x*cameraRight+p.y*cameraUpward);

        // enforce that it is orthonormal
        GramSchmidt42(ori,dir);

        // initialize color
        vec3 col = vec3(0);

        // acquire hit with scene
        hit h = scene(ori, dir, col);

        // diffuse
        //float d = max(dot(LIGHT_VECTOR, h.n),0.);
        //col *= (.25 + d*.75);

        // fog
        //float m = max(dot(dir, LIGHT_VECTOR),0.)*.5;
        //vec3 skyColor = FOG_COLOR + m*LIGHT_COLOR;
        //col = mix(col, skyColor, smoothstep(0., FOG_DISTANCE, h.t));

        // gamma correction
        //col = pow(col,vec3(1./2.2));

        // Output to screen
        fragColor = vec4(col,1.0);
    }
#endif