Shader "DuetCats/UI/IrisReveal"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Radius ("Iris Radius", Float) = 1.2
        _Feather ("Iris Feather", Range(0.001, 0.1)) = 0.015
        _Aspect ("Screen Aspect", Float) = 0.5625
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            float _Radius;
            float _Feather;
            float _Aspect;

            v2f vert(appdata_t input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.color = input.color;
                output.texcoord = input.texcoord;
                return output;
            }

            float4 frag(v2f input) : SV_Target
            {
                float2 centeredUv = input.texcoord - float2(0.5, 0.5);
                centeredUv.x *= _Aspect;
                float distanceFromCenter = length(centeredUv);
                float irisAlpha = smoothstep(_Radius, _Radius + _Feather, distanceFromCenter);
                return float4(0.0, 0.0, 0.0, irisAlpha * input.color.a);
            }
            ENDCG
        }
    }
}
