Shader "Universal Render Pipeline/UFO/CometTrailURP"
{
    Properties
    {
        _ColorHead ("Head Color (HDR)", Color) = (1,0,1,1)
        _ColorTail ("Tail Color (HDR)", Color) = (1,0,1,0)
        _Alpha     ("Alpha", Range(0,1)) = 1
        _MainTex   ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="UniversalForward" }

            // aditivo, sin ZWrite y doble cara (lo normal para trails brillantes)
            Blend One One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorHead;
                float4 _ColorTail;
                float  _Alpha;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // 1 = cabeza (cerca del emisor), 0 = cola; invierte si tu trail lo necesita
                half t = saturate(1.0 - IN.uv.y);
                half3 grad = lerp(_ColorTail.rgb, _ColorHead.rgb, t);
                half3 tex  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                half3 col  = grad * tex * _Alpha;
                return half4(col, 0); // aditivo
            }
            ENDHLSL
        }
    }
    FallBack Off
}
