float4x4 World;
float4x4 View;
float4x4 Projection;
texture Texture;
float LightPower;

// This sample uses a simple Lambert lighting model.
//float3 LightDirection = normalize(float3(-1, -1, -1));
float3 DiffuseLight = 0.05;
float3 AmbientLight = 0.05;


float AmbientIntensity = 1;
float4 AmbientColor : AMBIENT = float4(.5,.5,.5,1);

float3 LightDirection : Direction = float3(0,50,10);

sampler Sampler = sampler_state
{
    Texture = (Texture);
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Light : TEXCOORD0;
    float3 Normal : TEXCOORD1;
	float2 TextureCoordinate : TEXCOORD3;
};

VertexShaderOutput VertexShaderCommon(VertexShaderInput input, float4x4 instanceTransform)
{
	VertexShaderOutput output;

	//input.Position = instanceTransform.Position!!!

	input.Position.w = 1.0f;

    float4 worldPosition = mul(input.Position, instanceTransform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.Light = normalize(float3(0,5,0));
	output.Normal = normalize(worldPosition + input.Normal);
	//output.Normal = normalize(mul(input.Normal, (float3x3)instanceTransform));

    // Copy across the input texture coordinate.
    output.TextureCoordinate = input.TextureCoordinate;	

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(Sampler, input.TextureCoordinate)  * LightPower/10 * AmbientColor; //* saturate(dot(input.Light, input.Normal)) * 5; //* LightPower/10 * AmbientColor;
}

// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float4x4 instanceTransform : BLENDWEIGHT)
{
    return VertexShaderCommon(input, mul(World, transpose(instanceTransform)));
}

technique SKraft
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
