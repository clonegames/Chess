Shader "Chess/Board" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_GridSize("Grid Size", int) = 4
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;

		half _Glossiness;
		half _Metallic;
		int _GridSize;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c;
			fixed2 uv = fixed2(IN.uv_MainTex.x, IN.uv_MainTex.y) * _GridSize;
			if ((frac(uv.x) > 0.5 && frac(uv.y) < 0.5)|| (frac(uv.y) > 0.5 && frac(uv.x) < 0.5)) {
				c = _Color;
			}
			else {
				c = fixed4(1, 1, 1, 1) - _Color;
			}
			o.Albedo = c.rgb;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
