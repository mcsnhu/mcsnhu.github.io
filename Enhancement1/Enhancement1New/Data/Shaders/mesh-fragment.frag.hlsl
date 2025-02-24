struct DirectionalLight
{
    float3 Direction;
    float Intensity;
    float3 Color;
    float AmbientThreshold;
    float3 AmbientColor;
};

cbuffer UniformBlock : register(b0, space3)
{
    DirectionalLight MainLight;
    float3 CameraPosition;
};

Texture2D<float4> DiffseTexture : register(t0, space2);
SamplerState Sampler : register(s0, space2);

float4 main(float2 TexCoord: TEXCOORD0, float3 Normal: TEXCOORD1, float3 FragPosition : TEXCOORD2)
    : SV_Target0
{
    float3 surfaceColor = DiffseTexture.Sample(Sampler, TexCoord).rgb;

    // ambient light
    float3 ambientComponent = MainLight.AmbientThreshold * surfaceColor * MainLight.AmbientColor;

    // a simple directional light
    float3 normal = normalize(Normal);
    float3 lightDirection = normalize(MainLight.Direction);
    float diffuseFactor = saturate(dot(normal, lightDirection));
    float3 diffuseComponent = diffuseFactor * MainLight.Color * MainLight.Intensity;

    // compose the final result
    float3 finalColor = max(ambientComponent, diffuseComponent) * surfaceColor;

    return float4(finalColor, 1.0);
}
