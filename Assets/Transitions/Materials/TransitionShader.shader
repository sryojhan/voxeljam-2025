Shader "Hidden/TransitionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _TransitionGradient("Transition gradient", 2D) = "white" {}
        _Background("Background texture", 2D) = "black" {}

        _time("Time", Range(0, 1)) = 0

        [Toggle] _inverted ("Inverted", Float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _TransitionGradient;
            sampler2D _Background;

            float _time;
            float _inverted;

            float4 frag (v2f i) : SV_Target
            {
                //Coge el color del pixel de la pantalla
                float4 screenPixel = tex2D(_MainTex, i.uv);

                //Coge el color de la mascara para crear la transicion
                float transitionGradient = tex2D(_TransitionGradient, i.uv).r;

                //Coge el color del fondo de la transicion
                float4 background = tex2D(_Background, i.uv);

                //Offset para que el color completamente negro se perciba
                float offset = 0.001;

                //El nuevo valor corregido para comprobar la mascara del gradiente (limitado entre 0 y 1, y con un desfase añadido)
                float correctedTransitionGradient = clamp(transitionGradient, offset, 1 - offset);

                float gradientWithInversion = correctedTransitionGradient * (1 - _inverted) + (1 - correctedTransitionGradient) * _inverted;

                //Comprobacion de que el pixel se encuentre o no dentro del rango de la mascara de valores
                float isInsideMask = step(gradientWithInversion, _time);


                //Color final para pintar en la pantalla
                float4 returnValue = lerp(screenPixel, background, isInsideMask);
                return returnValue;
            }
            ENDCG
        }
    }
}
