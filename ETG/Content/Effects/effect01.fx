#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
	
};

float Time;

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	if(col.a > 0)
	{
		col.a = sin(Time + 0) * 0.5 + 0.5;
	}
		return col;
}
technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

/* Rainbow effect. 
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 col = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;

	if(col.a > 0.0f)
	{
		float4 shift;
		shift.r = sin(Time + 0) * 0.5 + 0.5; //Sin value will be between 0 to 1 
		shift.g = sin(Time + 2) * 0.5 + 0.5;
		shift.b = sin(Time + 3) * 0.5 + 0/5;
		shift.a = 1.0; // Preserve the alpha

	col = lerp(col, shift, 0.5);
	}
	return col;
}
*/

/* Clamp the rainbow to be dark gray to be black 
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 col = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;

	if(col.a > 0.0f)
	{
		float4 shift;
		float sinValue = sin(Time) * 0.5 + 0.5; // sinValue will be between 0 and 1

		sinValue = clamp(sinValue,0,0.3);

		shift.g = sinValue;
		shift.r = sinValue;
		shift.b = sinValue;
		shift.a = 1; // Preserve the alpha

	col = lerp(col, shift, 0.5);
	}
	return col;
}*/

/* //Nothing special fading out from game window
float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 col = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	if(col.a > 0)
	{
		col.a = sin(Time + 0) * 0.5 + 0.5;
	}
		return col;
}*/