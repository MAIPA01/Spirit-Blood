#ifndef ENEMY_BOOM_SHADER_HLSL
#define ENEMY_BOOM_SHADER_HLSL

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "GeometryShaderHelpers.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 uv : TEXCOORD0;
};

struct VertexOutput {
    float3 positionWS : TEXCOORD0;
    float2 uv : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
};

struct GeometryOutput {
    float3 positionWS : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float2 uv : TEXCOORD2;

    float4 positionCS : SV_POSITION;
};

TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_ST;
float _NoiseScale;
float _BoomPower;
float _FallPower;
float _FallDelay;
float _AnimationProgress;

/* RANDOM */
void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
{
    float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
    Out = lerp(Min, Max, randomno);
}

/* NOISE */
inline float unity_noise_randomValue (float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
}

inline float unity_noise_interpolate (float a, float b, float t)
{
    return (1.0-t)*a + (t*b);
}

inline float unity_valueNoise (float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = unity_noise_randomValue(c0);
    float r1 = unity_noise_randomValue(c1);
    float r2 = unity_noise_randomValue(c2);
    float r3 = unity_noise_randomValue(c3);

    float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
    float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
    float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3-0));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3-1));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3-2));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    Out = t;
}

VertexOutput Vertex(Attributes input) {
    VertexOutput output = (VertexOutput)0;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionWS = vertexInput.positionWS;

    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    output.normalWS = normalInput.normalWS;

    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    return output;
}

GeometryOutput SetupVertex(float3 positionWS, float3 normalWS, float2 uv) {
    GeometryOutput output = (GeometryOutput)0;
    output.positionWS = positionWS;
    output.normalWS = normalWS;
    output.uv = uv;
    output.positionCS = CalculatePositionCSWithShadowCasterLogic(positionWS, normalWS);
    return output;
}

void SetupAndOutputTriangle(inout TriangleStream<GeometryOutput> outputStream, VertexOutput a, VertexOutput b, VertexOutput c) {
    
    float x;
    float y;
    float z;
    Unity_RandomRange_float(20240201, 0, 1, x);
    Unity_RandomRange_float(20240201, 0, 1, y);
    Unity_RandomRange_float(20240201, 0, 1, z);

    float3 vec = normalize(float3(x, y, z));

    float3 fallVec = float3(0, -_FallPower, 0) * pow(_AnimationProgress - _FallDelay, 2) + float3(0, _FallPower * _FallDelay * _FallDelay, 0);

    float noise;
    Unity_SimpleNoise_float(a.uv, _NoiseScale, noise);
    float3 aPosWS = a.positionWS + _AnimationProgress * _BoomPower * noise * normalize((vec + a.normalWS) / 2) + fallVec;
    Unity_SimpleNoise_float(b.uv, _NoiseScale, noise);
    float3 bPosWS = b.positionWS + _AnimationProgress * _BoomPower * noise * normalize((vec + b.normalWS) / 2) + fallVec;
    Unity_SimpleNoise_float(c.uv, _NoiseScale, noise);
    float3 cPosWS = c.positionWS + _AnimationProgress * _BoomPower * noise * normalize((vec + c.normalWS) / 2) + fallVec;

    float3 centerWS = GetTriangleCenter(aPosWS, bPosWS, cPosWS);
    
    aPosWS = aPosWS * (1.f - _AnimationProgress) + centerWS * _AnimationProgress;
    bPosWS = bPosWS * (1.f - _AnimationProgress) + centerWS * _AnimationProgress;
    cPosWS = cPosWS * (1.f - _AnimationProgress) + centerWS * _AnimationProgress;

    outputStream.Append(SetupVertex(aPosWS, a.normalWS, a.uv));
    outputStream.Append(SetupVertex(bPosWS, b.normalWS, b.uv));
    outputStream.Append(SetupVertex(cPosWS, c.normalWS, c.uv));

    outputStream.RestartStrip();
}

[maxvertexcount(3)]
void Geometry(triangle VertexOutput inputs[3], inout TriangleStream<GeometryOutput> outputStream) {
    SetupAndOutputTriangle(outputStream, inputs[0], inputs[1], inputs[2]);
}

void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
{
    Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
}

float4 Fragment(GeometryOutput input) : SV_Target {
#ifdef SHADOW_CASTER_PASS
    return 0;
#else
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;

    lightingInput.normalWS = NormalizeNormalPerPixel(input.normalWS);

    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);

    float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;

    SurfaceData surfaceInput = (SurfaceData)0;
    surfaceInput.albedo = albedo;
    surfaceInput.specular = 1;
    surfaceInput.smoothness = 0.5;
    surfaceInput.emission = 0;
    surfaceInput.alpha = 1;

    return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
#endif
}

#endif