Shader "Custom/BlueButton"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite", 2D) = "white" {}

        _TopColor ("Top Color", Color) = (0.36,0.70,1.0,1)
        _BottomColor ("Bottom Color", Color) = (0.14,0.35,0.82,1)

        _Hover ("Hover", Range(0,1)) = 0
        _Pressed ("Pressed", Range(0,1)) = 0

        _HighlightStrength ("Highlight", Range(0,1)) = 0.12
        _ShadowStrength ("Shadow", Range(0,1)) = 0.15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            fixed4 _TopColor;
            fixed4 _BottomColor;

            float _Hover;
            float _Pressed;

            float _HighlightStrength;
            float _ShadowStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 元画像
                fixed4 sprite = tex2D(_MainTex, i.uv);

                // グラデーション
                fixed4 color = lerp(_BottomColor, _TopColor, i.uv.y);

                // ===== 上側のハイライト =====
                float highlight = smoothstep(0.55, 1.0, i.uv.y);
                color.rgb += highlight * _HighlightStrength;

                // ===== 下側の影 =====
                float shadow = smoothstep(0.0, 0.45, 1.0 - i.uv.y);
                color.rgb *= 1.0 - shadow * _ShadowStrength;

                // ===== Hover =====
                color.rgb *= lerp(1.0, 1.12, _Hover);

                // ===== Press =====
                color.rgb *= lerp(1.0, 0.88, _Pressed);

                // Sprite形状維持
                color *= sprite;

                // Button(Color Tint)対応
                color *= i.color;

                return color;
            }

            ENDCG
        }
    }
}