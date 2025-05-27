Shader "Custom/DebugLightReflect"
{
    Properties
    {
        _NormalMap("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 tangentSpaceLight : TEXCOORD1;
            };


            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3x3 tbn = float3x3(worldTangent, worldBinormal, worldNormal);
                o.tangentSpaceLight = mul(tbn, lightDir);
                o.uv = TRANSFORM_TEX(v.uv, _NormalMap);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Get normal from normal map (convert from [0,1] to [-1,1])
                fixed3 normal = tex2D(_NormalMap, i.uv).xyz * 2 - 1;

                // Normalize both normal and light direction in tangent space
                normal = normalize(normal);
                float3 lightDir = normalize(i.tangentSpaceLight);

                // Dot product represents how much light hits the surface
                float intensity = saturate(dot(normal, lightDir)); // clamp to [0,1]

                return fixed4(intensity, intensity, intensity, 1); // grayscale output
            }
            ENDCG
        }
    }
}
