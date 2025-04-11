Shader "Youregone/TestShader" {

    Properties{
        [Header(Surface Options)]
        [MainTexture] _ColorMap("Color", 2D) = "white" {}
        [MainColor] _ColorTint("Tint", Color) = (1, 1, 1, 1)
        _OffsetXSpeed("XSpeed", Float) = 0.0
        _OffsetYSpeed("YSpeed", Float) = 0.0
    }

    SubShader {

        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "LightMode" = "Universal2D"
        }

        Pass {
            Name "Test"

            HLSLPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "TestShaderUniversal2D.hlsl" 
            ENDHLSL
        }
    }
}