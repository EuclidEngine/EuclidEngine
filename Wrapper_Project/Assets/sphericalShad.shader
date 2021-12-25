Shader "Spherical Space Shader" {
    Properties {
        _Camera ("Camera", Vector) = (0,0,0,1)
        _SphereOrigin ("3-Sphere Origin", Vector) = (0,0,0,0)
        _SphereRadius ("3-Sphere Radius", Float) = 0
        [PerRendererData]_TileOrigin ("Tile Hyper-Origin", Vector) = (0,0,0,1)
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