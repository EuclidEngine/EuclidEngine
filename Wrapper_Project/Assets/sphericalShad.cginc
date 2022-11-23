#include "UnityCG.cginc"

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4x4, _GyrVec)
UNITY_INSTANCING_BUFFER_END(Props)

#define PI 3.14159265359

struct v2g {
    float4 worldPos : TEXCOORD1;
    float4 normal : NORMAL;
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    float dot : TEXCOORD2;
    fixed4 color : COLOR;
};
struct g2f {
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
    float dot : TEXCOORD2;
    fixed4 color : COLOR;
};
struct sphericalPos {
    float r;
    float3 angles;
};

float4 _Camera;
float4 _TileOrigin;
float4 _SphereOrigin;
float _EEWorldRadius = 1;
float _Height;// TODO
sampler2D _MainTex;
int _Activate;

float3 mobius_add(float3 v1, float3 v2, inout float3 n) {
    float3 c = cross(v2, v1);
    float l = 1 - dot(v1, v2);
    float3 t = v1 + v2;
    float4 v3 = normalize(float4(c, l));
    float3 q = cross(v3.xyz, n); q += q;
    n += v3.w * q + cross(v3.xyz, q);
    return (t * l + cross(c, t)) / (l * l + dot(c, c));
}

v2g vertex(float4 vertex : POSITION, float4 normal : NORMAL, float4 uv : TEXCOORD0)
{
    v2g o;
    o.uv = uv;
    o.normal = normal;
    // o.pos = o.worldPos = vertex;
    o.color = vertex;//1 * (normalize(vertex) * 0.8 + abs(normal) * 0.2);//normalize(sphericalPos) / 2 + 0.5;

    float4x4 view = UNITY_MATRIX_V;
    view._m03_m13_m23 = 0.0;
    float3 shiftV = mul(UNITY_MATRIX_V._m03_m13_m23, view);
    shiftV *= tan(0.5 / _EEWorldRadius * 1.0001 / 0.5774) / 1.5;

    float4 wpos = mul(unity_ObjectToWorld, vertex / _EEWorldRadius);
    float3 wnormal = UnityObjectToWorldNormal(normal);// / _EEWorldRadius;
    o.worldPos = wpos;

    wpos.xyz *= 1.0001;
    wpos.y = tan(wpos.y) * sqrt(1 + dot(wpos.xz, wpos.xz));
    //wpos.y = tan(wpos.y) * sqrt(1.0 + dot(wpos.xz, wpos.xz));// If useTanH

    float4x4 i_GyrVec = UNITY_ACCESS_INSTANCED_PROP(Props, _GyrVec);
    wpos.xyz /= sqrt(1 + dot(wpos.xyz,wpos.xyz)) + 1.0;
    wpos.xyz = mobius_add(wpos.xyz, i_GyrVec._m03_m13_m23, wnormal);
    wpos.xyz = mul(i_GyrVec, wpos.xyz);
    wnormal = mul(i_GyrVec, wnormal);
    wpos.xyz = mobius_add(wpos.xyz, shiftV, wnormal);
    o.dot = dot(wpos.xyz,wpos.xyz);
#if SKY
    wpos.xyz /= o.dot;
#endif
    wpos *= _EEWorldRadius;

    if (_Activate)
        // Euclidean pos to Screen pos
        o.pos = mul(UNITY_MATRIX_P, mul(view, wpos));
    else
        o.pos = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, vertex));
    return o;
}

g2f lerpV2f(g2f i1, g2f i2, float v)
{
    g2f o;
    o.pos = lerp(i1.pos, i2.pos, v);
    o.uv = lerp(i1.uv, i2.uv, v);
    o.dot = lerp(i1.dot, i2.dot, v);
    o.color = lerp(i1.color, i2.color, v);
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

    g2f v00 = {input[(rightAngle + 0) % 3].pos, input[(rightAngle + 0) % 3].uv, input[(rightAngle + 0) % 3].dot, input[(rightAngle + 0) % 3].color};
    g2f v30 = {input[(rightAngle + 1) % 3].pos, input[(rightAngle + 1) % 3].uv, input[(rightAngle + 1) % 3].dot, input[(rightAngle + 1) % 3].color};
    g2f v03 = {input[(rightAngle + 2) % 3].pos, input[(rightAngle + 2) % 3].uv, input[(rightAngle + 2) % 3].dot, input[(rightAngle + 2) % 3].color};
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

void fragment(v2g i, out fixed4 color : SV_Target, out float depth : SV_DEPTH)
{
#if GROUND
    if (i.dot > 2.0) discard;
    depth = 0.5 + i.pos.z*0.5;
    // color = fixed4(0,0.5+i.pos.z/2,0,1);
#elif SKY
    if (i.dot < 0.5) discard;
    depth = 0.5 - i.pos.z*0.5;
    // color = fixed4(0.5-i.pos.z/2,0,0,1);
#endif
    color = lerp(i.color, tex2D(_MainTex, i.uv.xy), 1);
}

#if false
    // See https://www.shadertoy.com/view/wtXBRH
    // S^3 is defined as all the points in R^4 with unit length.

    // for two points 'x,y' in 'S^3' their distance is the angle between their respective vectors:
    // d(x,y) := arccos( dot(x,y) )

    // a ray 'p(t)' starting at 'o' going towards to 'd', where (o,d) = 0, is:
    // p(t) = cos(t)*o + sin(t)*d,
#endif