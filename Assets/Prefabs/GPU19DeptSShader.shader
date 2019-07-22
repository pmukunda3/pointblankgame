// Aaron Lanterman, July 1, 2019
// Modified example from https://github.com/Unity-Technologies/PostProcessing/wiki/Writing-Custom-Effects

Shader "Hidden/Custom/GPU19DepthSShader"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

        float _Speed;
        float _Shift;

        float4 Frag(VaryingsDefault i) : SV_Target
        {

            float shh = _Shift*(cos(_Speed * _Time.y) + 1);
            float4 original; 
            float4 original_left = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - float2(1.0 / _ScreenParams.x,0));
            original.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord-float2(shh / _ScreenParams.x,shh / _ScreenParams.y)).r;
            original.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord+float2(shh / _ScreenParams.x,shh / _ScreenParams.y)).g;
            original.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord-float2(-shh / _ScreenParams.y,shh / _ScreenParams.x)).b;
			float4 dn_enc = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord);
            // float4 d_enc = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord);            
            // float depth = Linear01Depth(d_enc);
            float depth = dot(float2(1.0, 1/255.0),dn_enc.zw);
			
            float3 n = DecodeViewNormalStereo(dn_enc);
			float3 display_n = 0.5 * (1 + n);
            original.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord+n.x*float2(shh / _ScreenParams.x,shh / _ScreenParams.y)).r;
            original.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord+n.y*float2(shh / _ScreenParams.x,shh / _ScreenParams.y)).g;
            original.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord+n.z*float2(-shh / _ScreenParams.y,shh / _ScreenParams.x)).b;
            return float4(original.rgb,1);

    		//return(float4(lerp(original.rgb,depth.xxx,0.5*(cos(_Speed * _Time.y) - 1)),1));

        }

    ENDHLSL

    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
