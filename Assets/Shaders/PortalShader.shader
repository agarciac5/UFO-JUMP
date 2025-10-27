Shader "Unlit/Portal"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture (Circular Alpha)", 2D) = "white" {}

        _Color ("Base Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,0.5,0,1)

        _Speed ("Spin Speed", Range(-5,5)) = 1.0
        _PatternStrength ("Swirl Strength", Range(0,100)) = 40.0
        _DissolveAmount ("Dissolve Amount", Range(0.341,1)) = 0.5
        _PortalScale ("Portal Scale", Range(0.1,5)) = 1.0

        _ReverseSpin ("Reverse Spin", Float) = 0
        _RandomizeSpin ("Randomize Spin", Float) = 0

        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 3.0
    }

    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Portal"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

           
            TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);        SAMPLER(sampler_MaskTex);

            float4 _MainTex_ST;
            float4 _MaskTex_ST;

          
            float4 _Color;
            float4 _EmissionColor;
            float _Speed;
            float _PatternStrength;
            float _DissolveAmount;
            float _PortalScale;
            float _ReverseSpin;
            float _RandomizeSpin;
            float _EmissionIntensity;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvMask : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uvMask = TRANSFORM_TEX(IN.uv, _MaskTex);
                return OUT;
            }

           
            float2 RotateUV(float2 uv, float angle, float2 center)
            {
                float s = sin(angle);
                float c = cos(angle);
                uv -= center;
                float2 r = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                return r + center;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y * _Speed;
                if (_ReverseSpin > 0.5) t = -t;

                float2 center = float2(0.5, 0.5);
                float2 uv = IN.uv;

              
                float2 scaledUV = (uv - 0.5) * _PortalScale + 0.5;

               
                float spin = t + (_RandomizeSpin > 0.5 ? sin(uv.x * 10 + _Time.y) * 0.5 : 0);
                float2 rotatedUV = RotateUV(scaledUV, spin, center);

               
                float2 delta = rotatedUV - center;
                float radius = length(delta);
                float angle = atan2(delta.y, delta.x);

               
                angle += sin(radius * _PatternStrength * 0.1 + _Time.y * _Speed) * 0.5;

                float2 swirlUV = center + float2(cos(angle), sin(angle)) * radius;

               
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, swirlUV);
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.uvMask).r;

              
                clip(mask - 0.1);

                float dissolve = smoothstep(_DissolveAmount - 0.1, _DissolveAmount + 0.1, mask);

                
                float3 baseColor = texColor.rgb * _Color.rgb;
                float glow = pow(saturate(1.0 - mask), 2.0);
                float3 emission = _EmissionColor.rgb * glow * _EmissionIntensity;

                return float4(baseColor + emission, dissolve * _Color.a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
