Shader "Dvornik/Distort"
{
	Properties
	{
		_Refraction("Refraction", Range(0.00, 100.0)) = 1.0
		_DistortTex("Base (RGB)", 2D) = "white" {}
		_TimeDistortionPeriod("Time Distortion Period", float) = 20
		_TimeDistortionEffect("Time Distortion Effect", float) = .05
	}

	SubShader
	{

		Tags{ "RenderType" = "Transparent" "Queue" = "Overlay" }
		LOD 100

		GrabPass{}

		CGPROGRAM
		#pragma surface surf NoLighting
		#pragma vertex vert

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _GrabTexture : register(s0);
		sampler2D _DistortTex : register(s2);
		float _Refraction;

		float4 _GrabTexture_TexelSize;

		struct Input
		{
			float2 uv_DistortTex;
			float3 color;
			float3 worldRefl;
			float4 screenPos;
			INTERNAL_DATA
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.color = v.color;
		}

		float _TimeDistortionPeriod;
		float _TimeDistortionEffect;

		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 distort = tex2D(_DistortTex, IN.uv_DistortTex) * float3(IN.color.r,IN.color.g,IN.color.b);
			distort += (sin(_Time/_TimeDistortionPeriod+IN.screenPos.x))*_TimeDistortionEffect -.5;
			float2 offset = distort * _Refraction * _GrabTexture_TexelSize.xy;
			IN.screenPos.xy = offset * IN.screenPos.z + IN.screenPos.xy;
			float4 refrColor = tex2Dproj(_GrabTexture, float4(IN.screenPos.x, IN.screenPos.y, 0, IN.screenPos.z));
			o.Alpha = refrColor.a;
			o.Emission = refrColor.rgb;
		}
		ENDCG
	}
}