Shader "Custom/ShieldFresnel_Energy"
{
    Properties
    {
        _Color ("Base Color", Color) = (0, 1, 0, 0.2) // Verde translúcido
        _FresnelColor ("Fresnel Color", Color) = (0.3, 1.0, 0.3, 1) // Verde brillante en bordes
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 2
        _FresnelIntensity ("Fresnel Intensity", Range(0, 5)) = 2.5
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _PulseStrength ("Pulse Strength", Range(0, 2)) = 0.6
        _EmissionBoost ("Emission Boost", Range(0, 5)) = 2.5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Cull back
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            float4 _Color;
            float4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;
            float _PulseSpeed;
            float _PulseStrength;
            float _EmissionBoost;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 norm = normalize(i.normal);

                // --- Fresnel básico ---
                float fresnel = pow(saturate(1.0 - dot(viewDir, norm)), _FresnelPower);
                float4 fresnelCol = _FresnelColor * (fresnel * _FresnelIntensity);

                // --- Pulso dinámico (efecto de respiración energética) ---
                float pulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);
                float pulseAmt = lerp(1.0, 1.0 + _PulseStrength, pulse);

                // --- Color y brillo ---
                float3 baseCol = _Color.rgb * 0.5;
                float3 finalCol = baseCol + fresnelCol.rgb * pulseAmt * _EmissionBoost;

                // --- Transparencia controlada ---
                float alpha = saturate(_Color.a + fresnel * 0.6);
                alpha *= 0.2; // Más transparente aún

                return float4(finalCol, alpha);
            }
            ENDCG
        }
    }
}
