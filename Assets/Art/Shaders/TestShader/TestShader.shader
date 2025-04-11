Shader "Youregone/TestShader" {

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

            #include "TestShaderForward.hlsl" 
            ENDHLSL
        }
    }
}