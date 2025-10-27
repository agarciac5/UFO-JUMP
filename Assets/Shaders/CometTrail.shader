Shader "Universal Render Pipeline/CometTrailUnlit"
{
    Properties
    {
        _ColorHead    ("Head Color (HDR)", Color) = (6,5,3,1)
        _ColorTail    ("Tail Color (HDR)", Color) = (2,3,6,1)
        _EmissionBoost("Emission Boost", Float)   = 3.0
        _OverallAlpha ("Overall Alpha", Range(0,1)) = 1.0
        _TailTightness("Tail Tightness", Float)   = 1.6
        _EdgeFade     ("Edge Fade", Range(0,1))   = 0.7

        _NoiseTex     ("Noise (optional)", 2D)    = "gray" {}
        _NoiseScale   ("Noise Scale", Vector)     = (2,1,0,0)
        _NoiseSpeed   ("Noise Speed", Vector)     = (0.5,0,0,0)
        _Distortion   ("Distortion", Float)       = 0.08
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass
        {
            Name "FORWARD_UNLIT"
            Blend One One       // Additive
            ZWrite Off
            Cull Off            // two-sided
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0; // Trail/quad UV: U=largo, V=ancho
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorHead;
                float4 _ColorTail;
                float  _EmissionBoost;
                float  _OverallAlpha;
                float  _TailTightness;
                float  _EdgeFade;
                float4 _NoiseScale; // xy
                float4 _NoiseSpeed; // xy
                float  _Distortion;
            CBUFFER_END

            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vp = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vp.positionCS;
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Ruido paneado (opcional)
                float2 nUV = uv * _NoiseScale.xy + (_Time.x * _NoiseSpeed.xy);
                float nSample = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, nUV).r;
                nSample = lerp(0.6, 1.0, saturate(nSample)); // contraste suave

                // Máscara a lo largo: se apaga hacia la cola (U -> 1)
                float maskLong = pow(saturate(1.0 - uv.x), _TailTightness);

                // Máscara lateral: bordes suaves (V centrado)
                float vCentered = abs( (uv.y - 0.5) * 2.0 ); // 0 centro, 1 bordes
                float maskSide = smoothstep(1.0, 1.0 - _EdgeFade, 1.0 - vCentered);

                // Color de cabeza a cola
                float3 col = lerp(_ColorHead.rgb, _ColorTail.rgb, saturate(uv.x));

                // Alpha final
                float alpha = _OverallAlpha * maskLong * maskSide;

                // Emission (brillo)
                float3 emissive = col * _EmissionBoost * nSample;

                // Additive: el alpha no afecta la suma, pero lo devolvemos para debug/compat.
                return float4(emissive * alpha, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
