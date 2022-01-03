#include "UnityCG.cginc"

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

float4 _Camera;
float4 _TileOrigin;
float4 _SphereOrigin;
float _SphereRadius;
int _Activate;
/*
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
*/
// cos -sin
// sin  cos
float2x2 rotation2D(float2 v1, float2 v2)
{
    float l = length(v1) * length(v2);
    if (l == 0.f)
        return float2x2(1.f,0.f,0.f,1.f);
    float2x2 trig;
    trig._11 = dot(v1, v2) / l;
    trig._22 = trig._11;
    trig._21 = dot(float2(-v1.y,v1.x), v2) / l; // Perp dot product => dot(perp(v1),v2) = ||v1|| ||v2|| sin
    trig._12 = -trig._21;
    return trig;
}

float4 reverseStereographicalProjection(float4 pos, float4 tileOrigin, float4 sphereCenter, float sphereRadius)
{
    // QP => P' <==> (xq,yq,zq,wq) + k*(xp-xq, yp-yq, zp-zq, wp-wq) => (x',y',z',w')
    // ||s-P'|| = rad <==> (xs-x')^2 + (ys-y')^2 + (zs-z')^2 + (ws-w')^2 = rad^2
    // (xs-x')² = (xs - xq - k(xp-xq))² = (k(xp-xq) + (xq-xs))²
    // (xs-x')² = k²(xp-xq)² + k*2(xp-xq)(xq-xs) + (xq-xs)²
    float4 Q = float4(0,0,0,1+sphereRadius) + float4(0,0,0,1) * sphereRadius;
    float4 QP = pos - Q;
    float4 SQ = float4(0,0,0,1) * sphereRadius;//Q - float4(0,0,0,1+sphereRadius);

    // (Ak² + Bk + C) = rad^2
    float A = dot(QP, QP); // == length(QP)^2
    float B = 2 * dot(QP, SQ);
    // C = length(SQ)^2 = radius^2 ==> C - rad^2 = 0

    // det = B²-4AC
    // k = (-B{+-}sqrt(det))/2A
    // k = {0, -B/A}
    float coeff = -B / A;

    // https://fr.wikipedia.org/wiki/Rotation_en_quatre_dimensions#Constructions_des_matrices_de_rotation
    // Get the rotation matrix between the projection point and Q 
    float4 projPoint = sphereCenter - (normalize(tileOrigin) * sphereRadius - sphereCenter);
    float4 calcCenterProj = SQ;
    float4 centerProj = projPoint - sphereCenter;
    float4x4 rotate = {1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1};
    // True when both are 0 or none are 0
    if ((length(calcCenterProj.xy) == 0) ^ (length(centerProj.xy) != 0)) {
        rotate._11_12_21_22 = rotation2D(calcCenterProj.xy, centerProj.xy);
        rotate._33_34_43_44 = rotation2D(calcCenterProj.zw, centerProj.zw);
    } else if ((length(calcCenterProj.xz) == 0) ^ (length(centerProj.xz) != 0)) {
        rotate._11_13_31_33 = rotation2D(calcCenterProj.xz, centerProj.xz);
        rotate._22_24_42_44 = rotation2D(calcCenterProj.yw, centerProj.yw);
    } else if ((length(calcCenterProj.xw) == 0) ^ (length(centerProj.xw) != 0)) {
        rotate._11_14_41_44 = rotation2D(calcCenterProj.xw, centerProj.xw);
        rotate._22_23_32_33 = rotation2D(calcCenterProj.yz, centerProj.yz);
    }

    return mul(SQ + coeff * QP, rotate) + sphereCenter;
}

// https://en.wikipedia.org/wiki/Stereographic_projection#Generalizations
float4 stereographicalProjection(float4 pos, float4 projVect, float4 sphereCenter, float sphereRadius)
{
    // QP => P' (P' ∈ S, OQ ⊥ S)
    // dot(QO,QP) = length(QO)*length(QP)*cos(OQP)
    // cos(OQP) = 2*OQ/QP'
    // QP' = 2*OQ/cos(OQP)
    float4 projPoint = normalize(projVect) * sphereRadius;
    float cosQ = dot(pos-sphereCenter-projPoint, projPoint) / (sphereRadius * sphereRadius); // len(pos-sphereCenter) == len(projPoint) == sphereRadius
    float coeff = 2 * sphereRadius / cosQ;
    float4 newPos = sphereCenter + projPoint + coeff * (pos-sphereCenter - projPoint);
    newPos.w = 1;
    return newPos;
#if false
    if (pos.w == projVect.w) {
        return float4(0,0,0,0);
    } else {
        // QP => P' (P'.w = {0,1})
        // (xq,yq,zq,wq) + k*(xp-xq, yp-yq, zp-zq, wp-wq) => (x',y',z',{0,1})
        // wq + k(wp-wq) = {0,1}
        // k = ({0,1}-wq) / (wp-wq) = (wq-{0,1}) / (wq-wp)
        float coeff = (projPoint.w - wTargetedPlan) / (projPoint.w - pos.w);
        return projPoint + coeff * (pos - projPoint);
    }  
#endif
}

/*
    Reverse orthographic projection relativ to chunk center
    ~ Transformation position relativ to hypercenter of chunk ~
    Stereographic projection relativ to camera
*/

v2g vertex(float4 vertex : POSITION, float4 normal : NORMAL)
{
    v2g o;
    o.normal = normal;
    o.color = 1 * (normalize(vertex) * 0.8 + abs(normal) * 0.2);//normalize(sphericalPos) / 2 + 0.5;

    // Vertex pos to Chunk pos
    float4 wpos = mul(unity_ObjectToWorld, vertex);
    // Chunk pos to Spherical pos
    wpos = reverseStereographicalProjection(wpos, _TileOrigin, _SphereOrigin, _SphereRadius);
    o.worldPos = wpos;
    // Spherical pos to Euclidean pos
    //float4 cameraPos = reverseStereographicalProjection(_Camera, _Camera, _SphereOrigin, _SphereRadius);
    wpos = stereographicalProjection(wpos, _Camera, _SphereOrigin, _SphereRadius);//_SphereOrigin - ((normalize(_Camera)*_SphereRadius) - _SphereOrigin));

    if (_Activate)
        // Euclidean pos to Screen pos
        o.pos = mul(UNITY_MATRIX_VP, wpos);
    else
        o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, vertex));
    return o;
    //float4 worldPos = float4(mul(unity_ObjectToWorld, vertex).xyz / _Radius,1);
    //worldPos = worldToSpherical(worldPos);
    //worldPos = stereographicalProjection(worldPos.xwzy);
    ////if (length(worldPos.xyz) <= 1) {
    ////    worldPos.z = sqrt(1 - worldPos.x * worldPos.x - worldPos.y * worldPos.y);
    ////    //worldPos = float3(worldPos.xy / (1 - worldPos.z), 1 / _Radius);
    ////}
    ////float4 worldPos = mul(unity_ObjectToWorld, vertex) / _Radius;
    ////if (length(worldPos.xyz) <= 1) {
    ////    worldPos.w = -sqrt(1 - worldPos.x * worldPos.x - worldPos.y * worldPos.y - worldPos.z * worldPos.z);
    ////    //worldPos.y = 1 / worldPos.y;
    ////    worldPos = float4(worldPos.xyz / (1 - worldPos.w), 1 / _Radius);
    ////    //worldPos.y = 1 / worldPos.y;
    ////}
    //v2g o;
    //o.worldPos = worldPos;
    //o.normal = normal;
    //if (_Activate)
    //    o.pos = mul(UNITY_MATRIX_VP, float4(worldPos.xyz * _Radius, 1));
    //else
    //    o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, vertex));
    //o.color = normalize(vertex) * 0.8 + abs(normal) * 0.2;//normalize(sphericalPos) / 2 + 0.5;
    //return o;
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
    //OutputStream.Append(v01);
    //OutputStream.Append(v10);
    //OutputStream.Append(v11);
    //OutputStream.Append(v20);
    //OutputStream.Append(v21);
    OutputStream.Append(v30);
    //OutputStream.RestartStrip();
    // |/|/
    //OutputStream.Append(v01);
    //OutputStream.Append(v02);
    //OutputStream.Append(v11);
    //OutputStream.Append(v12);
    //OutputStream.Append(v21);
    //OutputStream.RestartStrip();
    // |/
    //OutputStream.Append(v02);
    OutputStream.Append(v03);
    //OutputStream.Append(v12);
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
#endif