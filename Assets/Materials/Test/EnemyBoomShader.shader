Shader "Patryk/EnemyBoomShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseScale("Noise Scale", Float) = 1
        _BoomPower("Boom Power", Float) = 10
        _FallPower("Fall Power", Float) = 5
        _FallDelay("Fall Delay", Range(0, 1)) = 0.5
        _AnimationProgress("Animation Progress", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma require geometry

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment

            #include "EnemyBoomShader.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            Cull Back

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma require geometry

            #pragma multi_compile_shadowcaster

            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment

            #define SHADOW_CASTER_PASS

            #include "EnemyBoomShader.hlsl"

            ENDHLSL
        }
    }
}
