Shader "Spherical Space Shader" {
    Properties {
        _Camera ("Camera", Vector) = (0,0,0,1)
        _SphereOrigin ("3-Sphere Origin", Vector) = (0,0,0,0)
        //_SphereRadius ("3-Sphere Radius", Float) = 1 // Defined globally on script
        //[PerRendererData]_TileOrigin ("Tile Hyper-Origin", Vector) = (0,0,0,1)
        //[PerRendererData]_GyroVectorMat ("Matrixed GyroVector", Matrix) = (1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Activate ("Activate", Int) = 1
    }

    SubShader {
        Pass {
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertex
            //#pragma geometry geometry
            #pragma fragment fragment
            #define GROUND 1
            #include "sphericalShad.cginc"
            ENDHLSL
        }
        Pass {
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertex
            //#pragma geometry geometry
            #pragma fragment fragment
            #define SKY 1
            #include "sphericalShad.cginc"
            ENDHLSL
        }
    }
}