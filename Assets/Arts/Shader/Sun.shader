Shader "Universal Render Pipeline/Sun"
{
    Properties
    {
        // 纹理
        _MainTex("Texture", 2D) = "white" {}
        // 菲涅尔指数
        _FresnelExp("Fresnel Exponent", Range(0, 10)) = 1
        // 边缘光颜色（HDR）
        [HDR]_FresnelColor("Fresnel Color", Color) = (1, 0, 0, 1)
        // 边缘光强度
        _FresnelStrength("Fresnel Strength", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URP关键设置
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 4.5
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            CBUFFER_START(UnityPerMaterial)
                float _FresnelExp;
                float4 _FresnelColor;
                float _FresnelStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // 转换顶点位置
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // 处理UV
                OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
                
                // 转换法线到世界空间
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                
                // 计算视图方向
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 归一化法线和视图方向
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);
                
                // 采样纹理
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                // 计算菲涅尔效应
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelExp);
                
                // 计算边缘光
                half3 fresnelColor = fresnel * _FresnelColor.rgb * _FresnelStrength;
                
                // 最终颜色（纹理颜色 + 边缘光）
                half3 finalColor = texColor.rgb + fresnelColor;
                
                // URP中需要确保颜色不超出范围
                finalColor = saturate(finalColor);
                
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        // 添加阴影投射Pass（可选，但推荐）
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
}