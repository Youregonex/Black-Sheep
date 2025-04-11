#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct appdata {
    float3 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 positionCS : SV_POSITION; 
    float2 uv : TEXCOORD0;
};

float4 _ColorTint;

TEXTURE2D(_ColorMap);
SAMPLER(sampler_ColorMap);
float4 _ColorMap_ST;
float _OffsetXSpeed;
float _OffsetYSpeed;

v2f Vertex(appdata input) {
    v2f output;
    
    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS);
    output.positionCS = positionInputs.positionCS;
    float2 timeOffset = float2(_Time.y * _OffsetXSpeed, _Time.y * _OffsetYSpeed);
    output.uv = TRANSFORM_TEX(input.uv, _ColorMap) + timeOffset;
    return output; 
};

float4 Fragment(v2f input) : SV_TARGET {
    float2 uv = input.uv;
    float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
    return colorSample * _ColorTint;
};