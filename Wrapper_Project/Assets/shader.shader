Shader "Spherical space" {
    Properties {
        _Origin ("Origin", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 0
        _Height ("Height", Float) = 0
        _Activate ("Activate", Int) = 1
    }

    SubShader {
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #define PI 3.14159265359

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };

            float4 _Origin;
            float _Radius;
            float _Height;
            int _Activate;

            float4 euclideanPosToSpherical(float4 pos)
            {
                const float len = _Radius * sqrt(2);
                pos.y /= 100;
                float3 facePos = 2 * pos.xyz / len;
                if (facePos.x < -3 || facePos.x > 3 || facePos.z < -3 || facePos.z > 3)
                    return float4(0,0,0,0);
                else if (facePos.x < -1) {
                    facePos.y = -facePos.x - 1;
                    facePos.x = -1 + pos.y / len;
                } else if (facePos.x > 1) {
                    facePos.y = facePos.x - 1;
                    facePos.x = 1 - pos.y / len;
                } else if (facePos.z < -1) {
                    facePos.y = -facePos.z - 1;
                    facePos.z = -1 + pos.y / len;
                } else if (facePos.z > 1) {
                    facePos.y = facePos.z - 1;
                    facePos.z = 1 - pos.y / len;
                } else
                    facePos.y /= 2;

                float3 vec = normalize(facePos - float3(0,1,0));
                return float4(vec * _Radius + float3(0,_Radius,0), pos.w);


                /*const float4 center = _Origin + float4(0,_Radius,0,0);
                const float len = _Radius * sqrt(2);
                const float len2 = sqrt(2) / 2;
                // x, y, z -> lattitude, longitude, profondeur
                if (pos.x < -1.5 * len || pos.x > 1.5 * len || pos.z < -1.5 * len || pos.z > 1.5 * len)
                    return float4(0,0,0,0);
                //pos.y = (pos.y - _Origin.y) * _Radius / len*2 + _Origin.y;
                //if (pos.y > len/2) pos.y = len/2;
                float4 facePos = pos / len;
                if (pos.x < -len) {
                    facePos.x = pos.y / len - 1;
                    facePos.y = -2 * (pos.x+len) / len;
                }
                //facePos.x = fmod(pos.x + 1.5 * len, len) - len/2;
                //facePos.z = fmod(pos.z + 1.5 * len, len) - len/2;

                float4 vec = normalize(facePos - _Origin);
                //return vec;
                //if (pos.x < -0.5 * len)
                //    return float4(center.x + vec.y, center.y - vec.x, center.z + vec.z, center.w + vec.w);
                //else if (pos.x > 0.5 * len)
                //    return float4(center.x - vec.y, center.y + vec.x, center.z + vec.z, center.w + vec.w);
                //else if (pos.z < -0.5 * len)
                //    return float4(center.x + vec.x, center.y - vec.z, center.z + vec.y, center.w + vec.w);
                //else if (pos.z > 0.5 * len)
                //    return float4(center.x + vec.x, center.y + vec.z, center.z - vec.y, center.w + vec.w);
                return _Origin + vec * _Radius;*/

                // x, z -> len, angle -> longitude, lattitude -> x, z, w
                /*float len = sqrt(pos.x*pos.x + pos.z*pos.z);
                float angle = atan2(pos.z, pos.x);

                float longitude = angle;
                // len = 2 * radius * tan(PI/4-lattitude/2)
                float lattitude = PI/2 - 2*atan(len / 2 / (_Radius-pos.y));

                float latRad = cos(lattitude) * (_Radius-pos.y);
                return float4(
                    cos(longitude) * latRad,
                    _Radius -  sin(lattitude) * (_Radius-pos.y),
                    sin(longitude) * latRad,
                    pos.w
                );*/
            }

            float4 sphericalToWorldPos(float4 pos)
            {
                // x, y, z, w -> x, y, z
                return pos;
            }
            
            v2f vert(appdata_base v)
            {
                v2f o;
                float4 sphericalPos;
                if (_Activate) {
                    sphericalPos = euclideanPosToSpherical(mul(unity_ObjectToWorld, v.vertex));
                    //sphericalPos.y += _Radius - sphericalPos.w;
                    //sphericalPos.w = 1;
                } else
                    sphericalPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, sphericalPos);
                o.color = normalize(sphericalPos) / 2 + 0.5;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDHLSL
        }
    }
}