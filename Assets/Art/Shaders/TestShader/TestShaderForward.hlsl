#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct appdata {
    float3 positionOS : POSITION;
};

struct v2f {
    float4 positionCS : SV_POSITION; 
};

v2f Vertex(appdata input) {
    v2f output;

    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS);
    output.positionCS = positionInputs.positionCS;

    return output; 
};

float4 Fragment(v2f input) : SV_TARGET {
    return float4(.7, .3, .5, 1);
};