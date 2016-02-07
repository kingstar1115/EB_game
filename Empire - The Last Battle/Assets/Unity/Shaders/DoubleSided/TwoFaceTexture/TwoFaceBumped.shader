Shader "DoubleSided/TwoFaceTextureBumped" 
{
	Properties
	 {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Face Front (RGB)", 2D) = "white" {}
		_SecTex("Face Back (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap Front", 2D) = "bump" {}
		_BumpMapSec ("Normalmap Back", 2D) = "bump" {}
	}
	SubShader
	 {
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecTex;
		sampler2D _BumpMap;
		sampler2D _BumpMapSec;
		fixed4 _Color;

		struct Input
		 {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float face : VFACE;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 frontTex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 backTex = tex2D(_SecTex, IN.uv_MainTex) * _Color;

			float3 normalFront = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			float3 normalBack = UnpackNormal(tex2D(_BumpMapSec, IN.uv_BumpMap));

			float _sign = sign(IN.face);
			float value = (_sign + 1.0f) / 2.0f;
			
			float4 alb = lerp(backTex, frontTex, value);
			o.Albedo = alb.rgb;
			o.Alpha = alb.a;

			float3 norm = lerp(normalBack, normalFront, value);

			o.Normal = norm * sign(IN.face);
		}
		ENDCG
	}

	Fallback "DoubleSided/Legacy Shaders/VertexLitCullOff"
}
