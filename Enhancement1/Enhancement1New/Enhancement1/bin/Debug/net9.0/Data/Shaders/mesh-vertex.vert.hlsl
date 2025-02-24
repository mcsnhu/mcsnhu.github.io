cbuffer UniformBlock : register(b0, space1)
{
    float4x4 MatrixTransform;
    float4x4 ModelTransform;
    float4x4 NormalMatrix;
};

struct Input
{
    float3 Position : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float3 Normal : TEXCOORD2;
};

struct Output
{
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 FragPosition : TEXCOORD2;
    float4 Position : SV_Position;
};

Output main(Input input)
{
    Output output;
    output.Position = mul(MatrixTransform, mul(ModelTransform, float4(input.Position, 1.0f)));
    output.TexCoord = input.TexCoord;
    output.Normal = mul(float3x3(NormalMatrix[0].xyz, NormalMatrix[1].xyz, NormalMatrix[2].xyz), input.Normal);
    output.FragPosition = mul(ModelTransform, float4(input.Position, 1.0f)).xyz;
    return output;
}
