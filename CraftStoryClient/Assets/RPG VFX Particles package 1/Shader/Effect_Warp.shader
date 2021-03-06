Shader "AppsTools/FastShader/Effect/Effect_Warp" {
    Properties {
        _Albedo ("_Albedo(_Albedo)", 2D) = "white" {}
        _Alpha ("_Alpha", Float ) = 0
        _warp ("_warp", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Alpha;
            uniform float _warp;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v)
			{
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.screenPos = o.pos;
                return o;
            }

            float4 frag(VertexOutput i) : COLOR {
              
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (float2(_Albedo_var.r,_Albedo_var.g)*_Albedo_var.a*_warp*i.vertexColor.a);

                 return fixed4(lerp(tex2D(_GrabTexture, sceneUVs).rgb, 0,_Alpha),1);
            }
            ENDCG
        }
        
    }
    FallBack "Legacy Shaders/VertexLit"

}
