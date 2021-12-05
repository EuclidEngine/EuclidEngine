Shader "Spherical Space Shader" {
    Properties {
        _Origin ("Origin", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 0
        _Height ("Height", Float) = 0
        _Activate ("Activate", Int) = 1
    }

    SubShader {
        Pass {
            HLSLPROGRAM
            #pragma vertex vertex
            #pragma geometry geometry
            #pragma fragment fragment
            #include "sphericalShad.cginc"
            ENDHLSL
        }
    }
}