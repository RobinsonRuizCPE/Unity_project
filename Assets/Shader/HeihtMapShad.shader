Shader "Custom/HeihtMapShad" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.0 
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_SecondTex("Grass text",2D) = "white" {}
		_ThirdTex("Sand text",2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#define PI 3.14159265358979

		sampler2D _MainTex;
		sampler2D _SecondTex;
		sampler2D _ThirdTex;
		float _amplification;
		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		// 2D Random
		float random(float2 st)
		{
			return frac(sin(dot(st.xy,
				float2(12.9898, 78.233)))
				* 43758.5453123);
		}

		// 2D Noise based on Morgan McGuire @morgan3d
		// https://www.shadertoy.com/view/4dS3Wd
		float noise(float2 st)
		{
			float2 i = floor(st);
			float2 f = frac(st);

			// Four corners in 2D of a tile
			float a = random(i);
			float b = random(i + float2(1.0, 0.0));
			float c = random(i + float2(0.0, 1.0));
			float d = random(i + float2(1.0, 1.0));

			// Smooth Interpolation

			// Cubic Hermine Curve.  Same as SmoothStep()
			float2 u = f * f*(3.0 - 2.0*f);
			// u = smoothstep(0.,1.,f);

			// Mix 4 coorners percentages
			return lerp(a, b, u.x) +
				(c - a)* u.y * (1.0 - u.x) +
				(d - b) * u.x * u.y;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			
			// Albedo comes from a texture tinted by color
			float esp = 0.01;// precision of normal calculation(central difference) 
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 hill_tex = tex2D(_SecondTex, IN.uv_MainTex);
			fixed4 sand_tex = tex2D(_ThirdTex, IN.uv_MainTex);
			float ns = noise(50.0*(IN.uv_MainTex*2.0 - 1.0));//2D noise pattern
			float nf = ns + 0.5*noise(500.0*(IN.uv_MainTex*2.0 - 1.0)) - 0.5;;

			nf = 0.01 * (nf - 0.5); //final noise for bondary randomisation
			float nsnow =ns - 1.0;
			//Central difference
			float espx = clamp(esp,0.0, min(IN.uv_MainTex.x,1.0 - IN.uv_MainTex.x));
			float espy = clamp(esp,0.0, min(IN.uv_MainTex.y, 1.0 - IN.uv_MainTex.y));
			float3 n = float3(tex2D(_MainTex, IN.uv_MainTex - float2(espx, 0.0)).r - tex2D(_MainTex, IN.uv_MainTex + float2(espx, 0.0)).r,
								(espx+espy),
								tex2D(_MainTex, IN.uv_MainTex - float2(0.0,espy)).r - tex2D(_MainTex, IN.uv_MainTex + float2(0.0, espy)).r);
			n = normalize(n);


			

			
			float lvl = c.r + 5.0*nf;
			lvl *= _amplification*1.5;
			

			half3 color = sand_tex;//half3(0.4, 0.2, 0.2);//Riverbed color
			half3 grass = hill_tex;//half3(0.3, lerp(0.2,1.0,c.r),0.1);

			color = lerp(color, grass, smoothstep(0.20*20.0,0.24*20.0,lvl));//Grass
			color = lerp(color, half3(0.95, 0.95, 1.0), smoothstep(0.58*20.0, 0.62*20.0f, lvl));//Snow

			/*
			if (lvl <= 1.0 && lvl > 0.6)
			{
				c.rgb = half3(0.8, 0.8, 0.8);
			}
			else if (lvl <= 0.6 && lvl > 0.3)
			{
				c.rgb = half3(0.1, 0.8, 0.1);
			}
			else
			{
				c.rgb = half3(0.6, 0.3, 0.3);
			}
			*/

			//steep dependant factor
			float p = dot(float3(0.0, 1.0, 0.0), n);
			o.Albedo = lerp(half3(0.4, 0.4,0.0),color, smoothstep(0.15, 0.25, p));
			//o.Albedo = float3(ns, ns, ns); //debug noise
			//o.Albedo = grass;
			//float xx = smoothstep(0.2, 0.25, p);//debug steep mix
			//o.Albedo = float3(xx,xx,xx);

			//o.Albedo = n; //debug normals
			//o.Albedo = color;
			o.Normal = n;
			//o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
