Shader "UI/Split_MetallicRed_FlatBlue"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Split Settings)]
        _SplitX ("分割位置 (X)", Range(0.0, 1.0)) = 0.5
        
        [Header(Left Side (Metallic Red))]
        _LeftColor("左側の色", Color) = (1, 0.1, 0.1, 1) // メタリックな赤
        _ShineSpeed("ハイライトの速度", Float) = 1.5
        
        [Header(Right Side (Flat Blue))]
        _RightColor("右側の色", Color) = (0.1, 0.3, 1.0, 1) // のっぺりした青

        // UIマスク対応のための必須プロパティ群
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestingMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _SplitX;
                half4 _LeftColor;
                half _ShineSpeed;
                half4 _RightColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.worldPosition = IN.positionOS;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 元の画像をサンプリング
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // もしピクセルが完全に透明なら、計算を飛ばして透明のまま返す
                if (texColor.a <= 0.0) return half4(0,0,0,0);


                // ==========================================
                // 1. 左半分：メタリックな赤の計算
                // ==========================================
                // 画像の明るさ（ルミナンス）を抽出して白黒にする
                half luminance = dot(texColor.rgb, half3(0.299, 0.587, 0.114));
                
                // コントラストを強め（2.0倍）にして赤色を掛け合わせる
                half3 metallicRed = _LeftColor.rgb * (luminance * 2.0);
                
                // 斜めに走るハイライト（キラッとした光）を数学的に作る
                // uv.x と uv.y を足すことで斜めのグラデーションにし、時間(_Time.y)で動かす
                half shineLine = frac(IN.uv.x + IN.uv.y - _Time.y * _ShineSpeed);
                // 光の幅を細く鋭くする（0.45〜0.5の間だけ光らせる）
                half shine = smoothstep(0.45, 0.5, shineLine) - smoothstep(0.5, 0.55, shineLine);
                
                // ベースの赤にハイライトを足す
                metallicRed += shine * 0.8;


                // ==========================================
                // 2. 右半分：のっぺりした青の計算
                // ==========================================
                // テクスチャの陰影（元の色）を完全に無視して、指定した青色をそのまま使う
                half3 flatBlue = _RightColor.rgb;


                // ==========================================
                // 3. 左右の合成
                // ==========================================
                // UVのX座標が _SplitX（デフォルト0.5）より大きいか判定
                // isRight は x >= 0.5 なら 1.0、未満なら 0.0 になる
                half isRight = step(_SplitX, IN.uv.x); 
                
                // lerpを使って、isRightの値に応じて左の色と右の色を切り替える
                half3 finalRGB = lerp(metallicRed, flatBlue, isRight);

                // 最後に元の画像のアルファ値（透明度による形）と、ImageのColorを掛け合わせる
                half4 finalColor = half4(finalRGB, texColor.a) * IN.color;

                return finalColor;
            }
        ENDHLSL
        }
    }
}