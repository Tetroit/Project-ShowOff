Shader "Hidden/Shader/Blink"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    #include "Assets/Shaders/HLSL/TetraLib/UVTransform.hlsl"
    #include "Assets/Shaders/HLSL/TetraLib/Noise.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity;

    TEXTURE2D_X(_MainTex);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // Note that if HDUtils.DrawFullScreen is not used to render the post process, you don't need to call ClampAndScaleUVForBilinearPostProcessTexture.

        UVInputs uvs = GetUVInputs(input.texcoord.xy);
        
#ifdef _DRAW_FULLSCREEN
        float2 uvScaled = ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord.xy);
#else
        float2 uvScaled = input.texcoord.xy;
#endif
        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvScaled).xyz;

        float horizontalFac = (0.5 - abs(0.5 - uvs.uvNormalized.x) * abs(0.5 - uvs.uvNormalized.x));
        
        _Intensity *= 0.5;

        float verticalFac = uvs.uvNormalized.y;
        float fac = 1 + max (1 - verticalFac, verticalFac) - 1 + _Intensity + (_Intensity - 0.5) * horizontalFac;
        //if (uvs.uvNormalized.y > 1 - _Intensity - (_Intensity - 0.5) * horizontalFac)
        
        sourceColor = lerp(sourceColor, float3(0.02, 0.01, 0), saturate(fac));

        return float4(sourceColor, Luminance(sourceColor).x);

    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Blink"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma multi_compile_local _ _DRAW_FULLSCREEN
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
