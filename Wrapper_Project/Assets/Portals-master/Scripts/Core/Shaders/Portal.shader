Shader "Custom/Portal"
{
    Properties
    {
        _InactiveColour ("Inactive Colour", Color) = (1, 1, 1, 1)
        _Curvature("Curvature", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _InactiveColour;
            uniform float _Curvature;
            int displayMask; // set to 1 to display texture, otherwise will draw test colour
            

            v2f vert (appdata v)
            {
                v2f o;

                float4 worldSpace = mul(unity_ObjectToWorld, v.vertex);
                worldSpace.xyz -= _WorldSpaceCameraPos.xyz;
                worldSpace = float4(0.0f, (worldSpace.z * worldSpace.z) * -_Curvature, 0.0f, 0.0f);
                v.vertex += mul(unity_WorldToObject, worldSpace);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                fixed4 portalCol = tex2D(_MainTex, uv);
                return portalCol * displayMask + _InactiveColour * (1-displayMask);
            }
            ENDCG
        }
    }
    Fallback "Standard" // for shadows
}
