// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/grasscloud"{

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Grassheight("Grass Heigth",Float) = 0.2
		_Grasswidth("Grass width",Float) = 0.5
		_CutOff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			Pass{
			Cull Off
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma geometry geom

				#include "UnityCG.cginc"

				struct v2g
				{
					float4 pos : SV_POSITION;
					float3 norm : NORMAL;
					float2 uv : TEXCOORD0;
					float3 color : TEXCOORD1;
				};

				struct g2f
				{
					float4 pos : SV_POSITION;
					float3 norm : NORMAL;
					float2 uv : TEXCOORD0;
					float3 diffuseColor : TEXCOORD1;
					float4 worldSpacePos : TEXCOORD2;
				};
				bool black_edge;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				half _Grassheight;
				half _Grasswidth;
				
				half _CutOff;
				v2g vert(appdata_full v)
				{
					v2g o;
					o.pos = v.vertex;
					o.norm = v.normal;
					o.uv = v.texcoord;
					o.color = tex2Dlod(_MainTex,v.texcoord).rgb;

					return o;
				}


				[maxvertexcount(4)]
				void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
				{
					float3 lightPosition = _WorldSpaceLightPos0;

					float3 perpendicularAngle = float3(1, 0, 0); // perpendiculaire à y
					float3 faceNormal = cross(perpendicularAngle, IN[0].norm);
					float3 v0 = IN[0].pos.xyz - float3(0,3.0f,0); //ground
					float3 v1 = IN[0].pos.xyz + IN[0].norm * _Grassheight; //above ground
					float3 color = (IN[0].color);

						g2f OUT;OUT.worldSpacePos = mul(unity_ObjectToWorld,IN[0].pos ) ;
						OUT.pos = UnityObjectToClipPos(v0 + perpendicularAngle * 0.9 * _Grassheight);
						OUT.norm = faceNormal;
						OUT.diffuseColor = color;
						OUT.uv = float2(1, 0);
						triStream.Append(OUT);

						OUT.pos = UnityObjectToClipPos(v0 - perpendicularAngle * 0.5 * _Grasswidth);
						OUT.norm = faceNormal;
						OUT.diffuseColor = color;
						OUT.uv = float2(0, 0);
						triStream.Append(OUT);

						OUT.pos = UnityObjectToClipPos(v1 + perpendicularAngle * 0.9 * _Grassheight);
						OUT.norm = faceNormal;
						OUT.diffuseColor = color;
						OUT.uv = float2(1, 1);
						triStream.Append(OUT);

						OUT.pos = UnityObjectToClipPos(v1 - perpendicularAngle * 0.5 * _Grasswidth);
						OUT.norm = faceNormal;
						OUT.diffuseColor = color;
						OUT.uv = float2(0, 1);
						triStream.Append(OUT);
						


					
					
					
				}

				fixed4 frag(g2f IN) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, IN.uv);
				fixed Dist = length(IN.worldSpacePos.xyz - _WorldSpaceCameraPos);
				
				if (c.a < 0.5  && Dist < 100.0f) {
					c.rgb = c.rgb /  (4.0f - 0.3*(Dist / 10));
					c.a -= 0.1*(Dist / 100);
				}
				clip(c.a - _CutOff);
				return c;// float4(IN.diffuseColor.rgb, 1.0f);
				}

				ENDCG

			  }
		}

}