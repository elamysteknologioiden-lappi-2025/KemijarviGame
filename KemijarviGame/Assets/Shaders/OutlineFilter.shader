Shader "Unlit/OutlineFilter"{
	Properties {
	    [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}
		_Tickness ("Line Thickness", Range(0.0005, 0.5)) = 0.01
	}
	SubShader {
		Pass{
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Tickness;
            struct Attributes{
                float4 position		    : POSITION;
                float2 uv               : TEXCOORD0;
            };
            struct Varyings{
                float2 uv				: TEXCOORD0;
                float4 vertex			: SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float SampleDepth(float2 uv){
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
#else
                return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
#endif
            }
            
            float EdgeDetect (float2 aUv) {
                float2 tickness = float2(_Tickness, _Tickness);
                float horizontal = 0;
                float vertical = 0;
                horizontal += SampleDepth(aUv + float2(-1.0*2, -1.0) * tickness) *  1.0;
				horizontal += SampleDepth(aUv + float2( 1.0 * 2, -1.0) * tickness) * -1.0;
				horizontal += SampleDepth(aUv + float2(-1.0 * 2,  0.0) * tickness) *  2.0;
				horizontal += SampleDepth(aUv + float2( 1.0 * 2,  0.0) * tickness) * -2.0;
				horizontal += SampleDepth(aUv + float2(-1.0 * 2,  1.0) * tickness) *  1.0;
				horizontal += SampleDepth(aUv + float2( 1.0 * 2,  1.0) * tickness) * -1.0;
				vertical += SampleDepth(aUv + float2(-1.0 * 2, -1.0) * tickness) *  1.0;
				vertical += SampleDepth(aUv + float2( 0.0 * 2, -1.0) * tickness) *  2.0;
				vertical += SampleDepth(aUv + float2( 1.0 * 2, -1.0) * tickness) *  1.0;
				vertical += SampleDepth(aUv + float2(-1.0 * 2,  1.0) * tickness) * -1.0;
				vertical += SampleDepth(aUv + float2( 0.0 * 2,  1.0) * tickness) * -2.0;
				vertical += SampleDepth(aUv + float2( 1.0 * 2,  1.0) * tickness) * -1.0;
                return sqrt(horizontal * horizontal + vertical * vertical);
            }
            
            Varyings vert(Attributes aInput){
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(aInput.position.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = aInput.uv;
                
                return output;
            }

			float4 alphaBlend(float4 aTop, float4 aBottom){
				float3 color = (aTop.rgb * aTop.a) + (aBottom.rgb * (1 - aTop.a));
				float alpha = aTop.a + aBottom.a * (1 - aTop.a);
				return float4(color, alpha);
			}

            half4 frag (Varyings aInput) : SV_Target {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float s = pow(1 - saturate(EdgeDetect(aInput.uv)),  250);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, aInput.uv)* s;;

				half4 yellowColor;
				yellowColor.r = 1;
				yellowColor.g = 0.88;
				yellowColor.b = 0.5;
				yellowColor.a = 1;

				float4 edgeColor = float4(yellowColor.rgb, yellowColor.a * s);
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, aInput.uv);
				return alphaBlend(col, edgeColor);

            }
            
			#pragma vertex vert
			#pragma fragment frag
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}
