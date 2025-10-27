Shader "Universal Render Pipeline/CometToonLit"
{
    Properties
    {
        _BaseMap        ("Albedo", 2D) = "white" {}
        _BaseColor      ("Base Color", Color) = (0.9,0.9,0.9,1)

        
        _RampThreshold  ("Ramp Threshold", Range(0,1)) = 0.5
        _RampSmooth     ("Ramp Smoothness", Range(0.001,0.5)) = 0.1

        
        _RimColor       ("Rim Color (HDR)", Color) = (1,1,1,1)
        _RimPower       ("Rim Power", Range(0.5,8)) = 2.5
        _RimStrength    ("Rim Strength", Range(0,2)) = 0.6

        
        _TrailTint      ("Trail Tint (HDR)", Color) = (0.6,0.8,1,1)
        _EmissionStrength("Emission Strength", Range(0,8)) = 3.0

        
        _OutlineColor   ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth   ("Outline Width", Range(0,0.02)) = 0.006
    }

    SubShader
    {
        Tags{
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        
        Pass
        {
            Name "ForwardToon"
            Cull Back
            ZWrite On
            ZTest LEqual
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float  _RampThreshold;
                float  _RampSmooth;

                float4 _RimColor;
                float  _RimPower;
                float  _RimStrength;

                float4 _TrailTint;
                float  _EmissionStrength;
            CBUFFER_END

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS  = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv          = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(GetCameraPositionWS() - IN.positionWS);

                
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(N, mainLight.direction));

                
                float edge1 = _RampThreshold - _RampSmooth;
                float edge2 = _RampThreshold + _RampSmooth;
                float ramp  = smoothstep(edge1, edge2, NdotL);

                
                float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                
                float3 lit = albedo.rgb * (0.25 + 0.75 * ramp) * mainLight.color;

                
                float rim = pow(1.0 - saturate(dot(N, V)), _RimPower) * _RimStrength;
                float3 rimCol = _RimColor.rgb * rim;

                
                float3 emission = _TrailTint.rgb * _EmissionStrength;

                float3 finalCol = lit + rimCol + emission;

                return float4(finalCol, 1.0);
            }
            ENDHLSL
        }

        
        Pass
        {
            Name "Outline"
            Cull Front      
            ZWrite On
            ZTest LEqual
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float  _OutlineWidth;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                
                float3 posOS = IN.positionOS.xyz + normalize(IN.normalOS) * _OutlineWidth;
                float3 posWS = TransformObjectToWorld(posOS);
                OUT.positionHCS = TransformWorldToHClip(posWS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
