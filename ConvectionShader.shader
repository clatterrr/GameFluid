Shader "Unlit/ConvectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CheckerTex("Texture", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CheckerTex;
            float4 _MainTex_ST;
            uniform float _ArrowRotation[256];
            uniform float _ArrowScale[256];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float uvx, uvy;

            int _TexelNumber = 16;
            int intuvx = floor(i.uv.x * _TexelNumber);
            int intuvy = floor(i.uv.y * _TexelNumber);
            float rad = _ArrowRotation[intuvy * _TexelNumber + intuvx] * 3.1415927f / 180.0f;
            float velocity = _ArrowScale[intuvy * _TexelNumber + intuvx];
            float cosRes = cos(rad);
            float sinRes = sin(rad);
            bool HighRes = false;
            if (HighRes)
            {
                int cosRange07 = floor(cosRes * 8.0f);
                int sinRange07 = floor(sinRes * 8.0f);
                cosRes = cosRange07 / 8.0f; //保证是1/8的倍数，从1/8到7/8
                sinRes = sinRange07 / 8.0f;
            }


            uvx = i.uv.x - 1.0f / 256.0f * cosRes * velocity;
            uvy = i.uv.y - 1.0f / 256.0f * sinRes * velocity;
            float4 col;
                if (uvx < 0)
                {
                    uvx += 1.0f;
                    col  = tex2D(_CheckerTex, float2(uvx, uvy));
                }
                else
                {
                    col =  tex2D(_MainTex, float2(uvx, uvy));
                }
                
                return col;
            }
            ENDCG
        }
    }
}