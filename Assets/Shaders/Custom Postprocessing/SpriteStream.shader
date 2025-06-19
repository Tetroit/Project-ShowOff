Shader "Hidden/Shader/SpriteStream"
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
    #include "Assets/Shaders/HLSL/TetraLib/Math.hlsl"

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
    float4 _ColorMask;
    float _Intensity;
    float2 _Scale;
    float2 _Offset;
    float _Twist;
    float _TwistOffset;
    float _TwistScale;
    float _Speed;

    float _MaskMin;
    float _MaskMax;


    TEXTURE2D_X(_MainTex);

    #ifdef _OVERLAY_ON
        TEXTURE2D(_Overlay);
    #endif

    float2 GetQuantizedUV(float2 uv, out float2 origin)
    {
        float2 uv_f = floor(uv * 0.5) * 2;
        //float2 uv_f = floor(uv);
        float2 uv_fr = frac(uv * 0.5) * 2;
        float2 off = nrand2(uv_f);
        uv_fr -= off;

        origin = uv_f;
        if (uv_fr.x > 1 || uv_fr.y > 1 || uv_fr.x < 0 || uv_fr.y < 0)
            return float2(-1, -1);
        else
            return uv_fr;
    }

    float CutUVAlpha (float2 uv, float alpha)
    {
        if (uv.x > 1 || uv.y > 1 || uv.x < 0 || uv.y < 0)
            return 0;
        else
            return alpha;
    }
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

        float3 uvCol = float3(frac(uvs.uvRatioMinC),0);

        float centerDist2 = dot(uvs.uvRatioMinC, uvs.uvRatioMinC);
        float centerDist = sqrt(centerDist2);

        float2 polar = float2(atan2(uvs.uvRatioMinC.y, uvs.uvRatioMinC.x), centerDist);
        float2 polarNorm = float2(polar.x / PI * 0.5 + 0.5, polar.y);

        float2 polarDtrt = float2 (frac(polarNorm.x + sin(polarNorm.y * _TwistScale + _TwistOffset) * _Twist), polarNorm.y);

        if (_Scale.x <= 0) _Scale.x = 1;
        if (_Scale.y <= 0) _Scale.y = 1;
        #ifdef _OVERLAY_ON
            float4 color = float4(0,0,0,1);
            float2 scaledPolar = polarDtrt * _Scale - float2(0, _Time.y * _Speed);

            float2 origin1;

            float4 color1 = float4(0,0,0,1);

            float2 spawnUV1 = GetQuantizedUV(scaledPolar + _Offset, origin1);
            
            if (origin1.x > _Scale.x-2 - floor(_Offset.x))
            {
                spawnUV1 = GetQuantizedUV(scaledPolar - float2(_Scale.x, 0) + _Offset, origin1);
                if (origin1.x >= -ceil(_Offset.x))
                    color1.a = 1;
                else 
                    color1.a = 0;
            }
            float2 origin2;

            float4 color2 = float4(0,0,0,1);

            float2 spawnUV2 = GetQuantizedUV(scaledPolar, origin2);
            
            if (origin2.x > _Scale.x-2)
            {
                spawnUV2 = GetQuantizedUV(scaledPolar - float2(_Scale.x, 0), origin2);
                if (origin2.x >= 0)
                    color2.a = 1;
                else 
                    color2.a = 0;
            }
            //float2 spawnUV2 = GetQuantizedUV(scaledPolar + _Scale * 0.25, origin2);
            //if (origin2.x < 0) 
            //{
            //    color.b = 1;
            //    spawnUV2 = GetQuantizedUV(scaledPolar  + _Scale * 0.25 + float2(_Scale.x, 0), origin2);
            //}

            color1 = SAMPLE_TEXTURE2D(_Overlay, s_linear_clamp_sampler, spawnUV1);
            color2 = SAMPLE_TEXTURE2D(_Overlay, s_linear_clamp_sampler, spawnUV2);
            //color.a = CutUVAlpha(spawnUV1, color.a);
            //color2.a = CutUVAlpha(spawnUV2, color2.a);
            
            color1.a = CutUVAlpha(spawnUV1, color1.a);
            color2.a = CutUVAlpha(spawnUV2, color2.a);
            color.a = saturate (color1.a + color2.a);
            color.rgb = saturate(color1.rgb * color1.a) + saturate(color2.rgb * color2.a) ;
        #else   
            float4 color = float4(polarDtrt, 0, 1);
        #endif

        color *= _ColorMask;
        //float3 color = lerp(sourceColor, Luminance(sourceColor), _Intensity) * float3(uvRatio,1);
        color.a *= invLerpByMinAndMaxClamped(centerDist, _MaskMin, _MaskMax) * _Intensity;

        return float4(lerp (sourceColor, color, color.a), 1);

        //float2 layer1 = GetQuantizedUV(polarNorm * float2(20, 20));
        //layer1 = saturate(layer1);
        //float2 layer2 = GetQuantizedUV(polarNorm * float2(20, 20) - float2(5, 5));
        //layer2 = saturate(layer2);
        //return float4(layer1 + layer2, 0,1);
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
                #pragma multi_compile_local _ _OVERLAY_ON
                #pragma multi_compile_local _ _DRAW_FULLSCREEN
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
