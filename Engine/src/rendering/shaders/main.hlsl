cbuffer ConstantBuffer : register(b0)
{
    matrix World;
    matrix View;
    matrix Projection;
};

Texture2D shaderTexture : register(t0);
SamplerState samplerState : register(s0);

struct VS_INPUT
{
    float3 Pos : POSITION;
    float3 Normal : NORMAL;
    float2 Tex : TEXCOORD0;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float3 Normal : NORMAL;
    float2 Tex : TEXCOORD0;
};

PS_INPUT VSMain(VS_INPUT input)
{
    PS_INPUT output;
    
    // Transform position by world, view, and projection matrices
    float4 worldPosition = mul(float4(input.Pos, 1.0f), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Pos = mul(viewPosition, Projection);
    
    // Pass normal and texture coordinates
    output.Normal = mul(input.Normal, (float3x3)World); // Transform the normal by the world matrix (without translation)
    output.Tex = input.Tex;

    return output;
}

float4 PSMain(PS_INPUT input) : SV_TARGET
{
    // Sample the texture at the given UV coordinates
    float4 texColor = shaderTexture.Sample(samplerState, input.Tex);

    // Simple diffuse lighting (assuming light direction is along z-axis)
    float3 lightDir = normalize(float3(0.0f, 0.0f, 1.0f));
    float diff = max(dot(input.Normal, lightDir), 0.0f);  // Basic Lambertian diffuse shading
    
    // Apply diffuse lighting to texture color
    return texColor * diff;
}
