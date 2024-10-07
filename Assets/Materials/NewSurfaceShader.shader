Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _FrontTex ("Front Texture", 2D) = "white" {}
        _BackTex ("Back Texture", 2D) = "white" {}
        _LeftTex ("Left Texture", 2D) = "white" {}
        _RightTex ("Right Texture", 2D) = "white" {}
        _UpTex ("Up Texture", 2D) = "white" {}
        _DownTex ("Down Texture", 2D) = "white" {}
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

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            sampler2D _FrontTex;
            sampler2D _BackTex;
            sampler2D _LeftTex;
            sampler2D _RightTex;
            sampler2D _UpTex;
            sampler2D _DownTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz; // La dirección en el espacio del objeto.
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir); // Normalizamos la dirección.

                // Variables para las coordenadas UV
                float2 uv;

                // Frente y reverso
                if (abs(dir.z) > abs(dir.x) && abs(dir.z) > abs(dir.y))
                {
                    if (dir.z > 0) 
                    {
                        // Cara Frontal
                        uv = (dir.xy + 1.0) * 0.5; // Mapeo correcto para la cara frontal
                        return tex2D(_FrontTex, uv); 
                    }
                    else 
                    {
                        // Cara Posterior
                        uv = (float2(-dir.x, dir.y) + 1.0) * 0.5; // Invertimos x para la cara trasera
                        return tex2D(_BackTex, uv); 
                    }
                }
                // Lados (Derecha e Izquierda)
                else if (abs(dir.x) > abs(dir.y))
                {
                    if (dir.x > 0) 
                    {
                        // Cara Derecha
                        uv = (float2(-dir.z, dir.y) + 1.0) * 0.5; // Ajustamos para el lado derecho
                        return tex2D(_RightTex, uv);
                    }
                    else 
                    {
                        // Cara Izquierda
                        uv = (float2(dir.z, dir.y) + 1.0) * 0.5; // Ajustamos para el lado izquierdo
                        return tex2D(_LeftTex, uv);
                    }
                }
                // Arriba y Abajo
                else
                {
                    if (dir.y > 0) 
                    {
                        // Cara Superior
                        uv = (float2(dir.x, -dir.z) + 1.0) * 0.5; // Ajustamos para la parte superior
                        return tex2D(_UpTex, uv);
                    }
                    else 
                    {
                        // Cara Inferior
                        uv = (float2(dir.x, dir.z) + 1.0) * 0.5; // Ajustamos para la parte inferior
                        return tex2D(_DownTex, uv);
                    }
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
