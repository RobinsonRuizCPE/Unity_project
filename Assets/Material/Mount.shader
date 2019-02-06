
Shader "ShaderMan/Mount"{

	Properties{
	_MainTex("Albedo (RGB)", 2D) = "white" {}
	_SecondTex("Hill Tex",2D) = "white"{}
	_ThirdTex("Sand Tex",2D) = "white"{}
	_Color("Color", Color) = (1,1,1,1)
	_Glossiness("Smoothness", Range(0,1)) = 0.5
	_Metallic("Metallic", Range(0,1)) = 0.0

	}

		SubShader
	{
	Tags{ "RenderType" = "Opaque" }
	CGPROGRAM
#pragma surface surf Standard fullforwardshadows
		//#pragma vertex vert
	#pragma target 3.0
		//#pragma fragment frag
		#include "UnityCG.cginc"


			// Use shader model 3.0 target, to get nicer looking lighting


		#define PI 3.14159265358979


		//License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

	#define FOG 100.
	#define PRE .01

	#define SUN normalize(fixed3(0,4,1))
		struct VertexInput {
		fixed4 vertex : POSITION;
		fixed2 uv : TEXCOORD0;
		fixed4 tangent : TANGENT;
		fixed3 normal : NORMAL;
		//VertexInput
		};


		struct VertexOutput {
		fixed4 pos : SV_POSITION;
		fixed2 uv : TEXCOORD0;
		//VertexOutput
		};

		//Variables
	float4 _iMouse;
	half _Glossiness;
	half _Metallic;
	sampler2D _SecondTex;
	sampler2D _MainTex;
	sampler2D _ThirdTex;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};
	fixed height(fixed2 P)
	{
		fixed H = 8.*tex2D(_MainTex,P / 96.).r +
				  2.*tex2D(_MainTex,P / 25.).r +
				  .1*tex2D(_MainTex,P / 2.).r;
		return min(H - .1*P.y,5.02 + .02*pow(cos(3.*H - 3.*P.y),2.));//+_Time.y
	}

	fixed4 raymarch(fixed4 P,fixed3 R)
	{
		P = fixed4(P.xyz + R * 2.,2);
		fixed E = 1.;
		[unroll(100)]
	for (int i = 0; i < 300; i++)
		{
			P += fixed4(R,1)*E;
			fixed H = height(P.xy);
			E = clamp(E + (H - P.z) - .5,E,1.);
			if (H - E * .6 < P.z)
			{
				P -= fixed4(R,1)*E;
				E *= .7;
				if (E < PRE*P.w / FOG) break;
			}
		}
		return P;
	}

	fixed shadow(fixed4 P,fixed3 R)
	{
		fixed S = 0.;
		P = fixed4(P.xyz,0);
		fixed E = .5;
		[unroll(100)]
	for (int i = 0; i < 10; i++)
		{

			P += fixed4(R,1)*E;
			fixed H = height(P.xy);
			if (H < P.z)
			{
				S = min((H - P.z)*9.,S);
			}
		}
		return S;
	}

	fixed bump(fixed2 P)
	{
		return tex2D(_SecondTex,P*4.).r*tex2D(_SecondTex,P).r*tex2D(_SecondTex,P / 4.).r;
	}

	fixed3 normal(fixed2 P)
	{
		fixed2 N = fixed2(1,0);
		N = height(P - N.xy*PRE)*-N.xy + height(P + N.xy*PRE)*N.xy + height(P - N.yx*PRE)*-N.yx + height(P + N.yx*PRE)*N.yx;
		return normalize(fixed3(N,-PRE));
	}

	fixed3 sky(fixed3 R)
	{
		fixed3 S = fixed3(1,.5,.2) / (dot(R,SUN)*99. + 99.5);
		fixed2 P = R.xy / sqrt(max(.1 - R.z,.1))*9.;
		fixed C = (cos(P.y + cos(P.x*.5 + cos(P.y)))*cos(P.y*.7)*.5 + .5)*min(abs(.1 - 6.*R.z),1.);
		return lerp(fixed3(.4,.7,.9),fixed3(1,1,1),C*C) + S;
	}

	fixed3 color(fixed4 P,fixed3 R)
	{
		fixed3 C = fixed3(0.2, 0.2, 0.2);
		fixed3 N = normal(P.xy);
		fixed3 I = reflect(R,N);
		fixed F = min(P.w / FOG,1.);
		fixed L = exp(shadow(P,-SUN) + dot(N,-SUN) - 1.);
		fixed3 S = lerp(fixed3(.3,.4,.5),fixed3(1,1,1),L) - bump(P.xy);
		if (P.y < 0.) {

		}
		C = fixed3(0.6*smoothstep(-PRE * (5.0 + 50.0*F), -PRE * 4.0, P.z - height(P.xy - PRE * fixed2(8, 2)))*sky(I)*min(I.z + 1., 1.)) + max(-I.z, 0.)*fixed3(.1, .2, .4);
		return lerp(C*S,sky(R),F*F);
	}


	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		fixed4 hill_tex = tex2D(_SecondTex, IN.uv_MainTex);
		fixed4 sand_tex = tex2D(_ThirdTex, IN.uv_MainTex);
		//fixed3 perpendicularAngle 
		fixed A = 3.*(.5 - (_iMouse.y / 1))*sign(_iMouse.y);//.6-.3*cos(_Time.y*.3);
		fixed B = 3.*(.5 - (_iMouse.x / 1))*sign(_iMouse.x);
		fixed3 R = mul(mul(normalize(fixed3(1, IN.uv_MainTex)), fixed3x3(cos(A), 0, sin(A), 0, 1, 0, sin(A), 0, -cos(A))), fixed3x3(cos(B), sin(B), 0, sin(B), -cos(B), 0, 0, 0, 1));
		//_Time.y,cos(_Time.y*.2),-3.+2.*cos(_Time.y*.3)// perpendiculaire à y
		//float3 faceNormal = cross(perpendicularAngle, IN.normal);
		// fixed grass_tex =tex2D (_GrassTex, IN.uv_GrassTex) * _Color;
		if (c.r <= 1 && c.r > 0.7)
		{
				o.Albedo = half3(0.8, 0.8, 0.8);

		}
		else if (c.r <= 0.7 && c.r > 0.3)
		{
			if (c.r > 0.65)
				o.Albedo = half3(0.0, 0.0, 0.0);
			else
				o.Albedo = hill_tex.rgb;
				//o.Albedo = half3(0.1, 0.8, 0.1);

		}

		else
		{
			//color( raymarch(fixed4(0, 0, 0, 0), R),R);// 
			o.Albedo = sand_tex.rgb;// half3(0.8, 0.6, 0.3);
		}
		//o.Normal = fixed3(0.0, 1.0, 0.0);										   -
		//o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = 0.5;// c.a;
	}

	/*
	 VertexOutput vert (VertexInput v)
	 {
	 VertexOutput o;
	 o.pos = UnityObjectToClipPos (v.vertex);
	 o.uv = v.uv;
	 //VertexFactory
	 return o;
	 }
	 fixed4 frag(VertexOutput i) : SV_Target
	 {

	 i.uv = (i.uv-1*.5)/1;
	 fixed A = 3.*(.5-(_iMouse.y/1))*sign(_iMouse.y);//.6-.3*cos(_Time.y*.3);
	 fixed B = 3.*(.5-(_iMouse.x/1))*sign(_iMouse.x);
	 fixed3 R = mul(mul(normalize(fixed3(1,i.uv)),fixed3x3(cos(A),0,sin(A),0,1,0,sin(A),0,-cos(A))),fixed3x3(cos(B),sin(B),0,sin(B),-cos(B),0,0,0,1));
	 fixed4 P = raymarch(fixed4(0, cos(_Time.y*.2),0,0),R);//_Time.y,cos(_Time.y*.2),-3.+2.*cos(_Time.y*.3)
	 fixed4 C = fixed4(color(P,R),1);
	 return C;

	 }*/
	 ENDCG
	}

}