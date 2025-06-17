Shader "Hidden/Shader/CustomPP"
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

        float2 uv = input.texcoord.xy;
        float2 uvScaled = ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord.xy);
        float2 uvFullscreen = uv * _ScreenParams.xy;
        float2 uvRatio = uvFullscreen * (_ScreenParams.ww - 1);
        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvScaled).xyz;

        float radialFac =  length(uvRatio - float2(0.5, 0.5)) * 1.0;

        float3 uvCol = float3(uvRatio,0);
        // Apply greyscale effect
        if (uvRatio.x > 0.99) uvCol = float3(1,1,1);
        if (uvRatio.x < 0.01) uvCol = float3(0,0,0);
        if (uvRatio.y > 0.99) uvCol = float3(1,1,1);
        if (uvRatio.y < 0.01) uvCol = float3(0,0,0);

        float3 color = uvCol;
        //float3 color = lerp(sourceColor, Luminance(sourceColor), _Intensity) * float3(uvRatio,1);

        return float4(color, 1);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Custom PP"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
