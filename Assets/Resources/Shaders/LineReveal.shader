Shader "Custom/LineReveal"
{
    Properties
    {
        _Color ("Line Color", Color) = (1,0,0,1)
        _Range ("Reveal Range", Float) = 3.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _Color;
            uniform float _Range;
            uniform float3 _MouseWorldPos;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.worldPos.xz, _MouseWorldPos.xz);
            
                // 감쇠 비율 계산 (0=가까움, 1=끝)
                float t = saturate(dist / _Range);
            
                // 알파는 중심에서 1, 외곽으로 갈수록 0
                float alpha = pow(1.0 - t, 2.0); // 감쇠 커브 (부드럽게 떨어지도록 제곱)
            
                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}
