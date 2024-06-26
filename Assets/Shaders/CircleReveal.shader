Shader "Custom/CircleRevealShader"
{
    Properties
    {
        _Cutoff ("Cutoff", Range(0, 1)) = 0.6
        _TransitionColor ("Transition Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _TransitionColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Cutoff;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float aspectRatio = _ScreenParams.y / _ScreenParams.x; // Calculate aspect ratio (height / width)
                float2 center = float2(0.5, 0.5);
                float2 adjustedUV = float2(i.uv.x, i.uv.y * aspectRatio); // Adjust UVs based on aspect ratio
                float2 adjustedCenter = float2(center.x, center.y * aspectRatio); // Adjust center based on aspect ratio
                float dist = distance(adjustedUV, adjustedCenter); // Use adjusted UVs and center for distance calculation
            
                // Calculate alpha value based on the threshold
                float alpha = smoothstep(_Cutoff - 0.05, _Cutoff, dist);
            
                // Return the transition color with modified alpha
                return fixed4(_TransitionColor.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
