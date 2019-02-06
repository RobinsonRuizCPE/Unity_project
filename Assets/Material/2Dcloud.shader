
Shader "ShaderMan/2Dcloud"
	{

	Properties{
	//Properties
	}

	SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

	Pass
	{
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	struct VertexInput {
    fixed4 vertex : POSITION;
	fixed2 uv:TEXCOORD0;
    fixed4 tangent : TANGENT;
    fixed3 normal : NORMAL;
	//VertexInput
	};


	struct VertexOutput {
	fixed4 pos : SV_POSITION;
	fixed2 uv:TEXCOORD0;
	//VertexOutput
	};

	//Variables

	fixed2 hash( fixed2 p ) // replace this by something better
{
	p = fixed2( dot(p,fixed2(127.1,311.7)),
			  dot(p,fixed2(269.5,183.3)) );

	return -1.0 + 2.0*frac(sin(p)*43758.5453123);
}

fixed noise( in fixed2 p )
{
    const fixed K1 = 0.366025404; // (sqrt(3)-1)/2;
    const fixed K2 = 0.211324865; // (3-sqrt(3))/6;

	fixed2 i = floor( p + (p.x+p.y)*K1 );
	
    fixed2 a = p - i + (i.x+i.y)*K2;
    fixed2 o = step(a.yx,a.xy);    
    fixed2 b = a - o + K2;
	fixed2 c = a - 1.0 + 2.0*K2;

    fixed3 h = max( 0.5-fixed3(dot(a,a), dot(b,b), dot(c,c) ), 0.0 );

	fixed3 n = h*h*h*h*fixed3( dot(a,hash(i+0.0)), dot(b,hash(i+o)), dot(c,hash(i+1.0)));

    return dot( n, fixed3(70.0,70.0,70.0) );
	
}

fixed fnoise(in fixed2 p)
{
	fixed f = 0.0;
    p *= 5.0;
    fixed2x2 m = fixed2x2( 1.6,  1.2, -1.2,  1.6 );
	f  = 0.5000*noise( p ); p = mul(m,p);
	f += 0.2500*noise( p ); p = mul(m,p);
	f += 0.1250*noise( p ); p = mul(m, p);
	f += 0.0625*noise( p ); p = mul(m, p);
    return(f);
}

// -----------------------------------------------

fixed cloud(in fixed2 uv, fixed cloudy, fixed time) {
    uv += 0.05*pow(sin(time+uv.x*4.0),2.0);
    fixed hh = 0.9+0.1*fnoise(uv*1.7+fixed2(time*-4.5,time*-3.7));
    fixed h = 0.9+0.1*fnoise(uv+fixed2(time*2.1,time*1.7));
    uv += fixed2(time*0.7,time*0.9);
	fixed d = cloudy*0.33+0.4*fnoise(uv*0.25)+0.2*h+0.5*hh;
    d = smoothstep(0.4,0.9,clamp(d*d,0.0,1.0));

    return (d*h);
}

fixed3 cloudlerp(fixed3 lightcolor,fixed3 skycolor,fixed density, fixed weight) {
    fixed3 cloudcolor = lerp(lightcolor,skycolor*0.5,smoothstep(0.2,0.8,density*0.5));
    fixed3 result = lerp(skycolor,cloudcolor,weight*smoothstep(0.2,0.9,density*3.0));
    return(result);
}





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
	
    fixed2 uv = i.uv / 1;
    fixed time = _Time.y*0.02;
    fixed cloudy = 0.5+0.6*sin(time*20.0);
    fixed3 lightcolor = fixed3(1.0,1.0,1.0);
    fixed3 skycolor =lerp(fixed3(111./255.0,178./255.0,197./255.0),fixed3(19./255.0,86./255.0,129./255.0),uv.y);
    fixed3 result = fixed3(0,0,0);
	fixed strato = cloud(uv*fixed2(0.44,0.88), cloudy*0.5, time*0.33);
    result = cloudlerp(lightcolor,skycolor,strato,0.33);
	fixed cumu = cloud(uv*2.0, cloudy, time);
    result = cloudlerp(lightcolor,result,cumu,0.9);
    //result = cloudlerp(lightcolor,skycolor,cumu,1.0);
    
	return fixed4( result, 1.0 );

	}
	ENDCG
	}
  }
}

