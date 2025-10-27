Shader "Universal Render Pipeline/ToonShaderURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0,1)) = 0.3
        _Strength ("Strength", Range(0,2)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _Detail ("Detail", Range(0.1,1)) = 0.3
    }

    SubShader
    {
        
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"}

        Pass
        {
          
            HLSLPROGRAM
          
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl" // Para TRANSFORM_TEX

         
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _Brightness;
            float _Strength;
            float4 _Color;
            float _Detail;

           
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
            };

            
            struct Varyings
            {
                float4 positionHCS  : SV_POSITION; 
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1; 
                
            };

          
            float ToonShading(float3 normalWS, float3 lightDirWS)
            {
                
                float NdotL = saturate(dot(normalWS, lightDirWS));
                
                
                return floor(NdotL / _Detail) * _Detail;
            }

           
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                return o;
            }

       
            float4 frag(Varyings i) : SV_Target
            {
             
                float3 normalWS = normalize(i.normalWS);
                
                Light mainLight = GetMainLight();
                float3 lightDirWS = mainLight.direction;
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float toonFactor = ToonShading(normalWS, lightDirWS);
                float4 finalColor = (texColor * _Color) * (toonFactor * _Strength + _Brightness);

                return finalColor;
            }
            ENDHLSL
        }
    }
 
}