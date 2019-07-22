// Aaron Lanterman, July 1, 2019
// Modified example from https://github.com/Unity-Technologies/PostProcessing/wiki/Writing-Custom-Effects

Shader "Hidden/Custom/GPU19ColorMissRepresentationBlendShader"
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

            float shh = _Shift;//*(cos(_Speed * _Time.y) + 1);
            float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float4 origina; 
			float4 dn_enc = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, i.texcoord);
            float depth = dot(float2(1.0, 1/255.0),dn_enc.zw)+1;//*10*(cos(_Speed * _Time.y) + 1);
			
            float3 n = DecodeViewNormalStereo(dn_enc);
			float3 display_n = 0.5 * (1 + n);

            origina.r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord+depth*float2(n.x*shh / _ScreenParams.x,n.z*shh / _ScreenParams.y)).r;
            origina.b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord-depth*float2(n.x*shh / _ScreenParams.x,n.z*shh / _ScreenParams.y)).b;
            origina.g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord+depth*float2(-n.x*shh / _ScreenParams.x,n.z*shh / _ScreenParams.y)).g;

            origina.r = lerp(original.r,origina.r,0.3*depth*(cos(_Speed * (_Time.y+2)) + 1));
            origina.g = lerp(original.g,origina.g,0.3*depth*(cos(_Speed * (_Time.y+4)) - 1));
            origina.b = lerp(original.b,origina.b,0.3*depth*(cos(_Speed * (_Time.y+6)) + 1));



            return float4(origina.rgb,1);

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
