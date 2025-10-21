Shader "Unlit/PortalGlow_Enhanced"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture (Circular Alpha)", 2D) = "white" {}

        _Color ("Base Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,0.5,0,1)

        _Speed ("Spin Speed", Range(-5,5)) = 1.0
        _PatternStrength ("Swirl Strength", Range(0,100)) = 40.0
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0.5
        _PortalScale ("Portal Scale", Range(0.1,5)) = 1.0

        _ReverseSpin ("Reverse Spin", Float) = 0
        _RandomizeSpin ("Randomize Spin", Float) = 0

        _EmissionIntensity ("Emission Intensity", Range(0,10)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvMask : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvMask = TRANSFORM_TEX(v.uv, _MaskTex);
                return o;
            }

            float2 RotateUV(float2 uv, float angle, float2 center)
            {
                float s = sin(angle);
                float c = cos(angle);
                uv -= center;
                float2 rotatedUV = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                return rotatedUV + center;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y * _Speed;
                if (_ReverseSpin > 0.5) t = -t;

                float spin = t + (_RandomizeSpin > 0.5 ? sin(i.uv.x * 10 + _Time.y) * 0.5 : 0);
                float2 center = float2(0.5, 0.5);

                // Escala general del portal (zoom interno)
                float2 scaledUV = (i.uv - 0.5) * _PortalScale + 0.5;

                // Rotación y remolino
                float2 rotatedUV = RotateUV(scaledUV, spin, center);
                float2 swirlUV = scaledUV + (rotatedUV - center) * (_PatternStrength * 0.01);

                // Texturas
                float4 texColor = tex2D(_MainTex, swirlUV);
                float mask = tex2D(_MaskTex, i.uvMask).r;

                // Corte circular (clip descarta píxeles negros de la máscara)
                clip(mask - 0.1);

                // Disolver borde
                float dissolve = smoothstep(_DissolveAmount - 0.1, _DissolveAmount + 0.1, mask);

                float3 baseColor = texColor.rgb * _Color.rgb;

                // Emisión con bloom
                float glow = pow(saturate(1.0 - mask), 2.0);
                float3 emission = _EmissionColor.rgb * glow * _EmissionIntensity;

                float alpha = dissolve * _Color.a;
                return float4(baseColor + emission, alpha);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Color"
}
