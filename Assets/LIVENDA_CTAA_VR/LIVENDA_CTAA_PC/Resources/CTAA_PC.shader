//LIVENDA CTAA V3 CINEMATIC TEMPORAL ANTI ALIASING
//Copyright Livenda Labs 2019
// Unity ASSET STORE V1.7


Shader "Hidden/CTAA_PC" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	
}

SubShader {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }
	Pass {

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma glsl
#pragma exclude_renderers d3d11_9x
#include "UnityCG.cginc"

            
float4 _MainTex_TexelSize;

uniform sampler2D _MainTex;
uniform sampler2D _Accum;
uniform sampler2D _Motion0;
float _CamMotion;

uniform sampler2D _CameraDepthTexture;
uniform float _motionDelta;
uniform float _motionDeltaDynamic;
uniform float _AdaptiveResolve;

float4 _ControlParams;
sampler2D_half _CameraMotionVectorsTexture;

float _AntiShimmer;
float4 _delValues;


float LLfsdg(float3 Color)
{
	return (Color.g * 2.0) + (Color.r + Color.b);
}

float JdrrggjgjjsiTTT4(float Color, float Exposure) 
{
	return 4.0 * rcp(Color * (-Exposure) + 1.0);
}

float sdfgguUUrtYeTTY(float Channel, float Exposure) 
{
	return Channel * JdrrggjgjjsiTTT4(Channel, Exposure);
}

float Lkmmmdr46dFFFFdeq(float3 Color) 
{
	
		 return dot(Color, float3(0.299, 0.587, 0.114));
	
		//return dot(Color, float3(0.2126, 0.7152, 0.0722));
	
}

inline float Lkmmmfggfdrtgte(float3 Color, float Exposure) 
{
	
	
	return rcp(LLfsdg(Color) * Exposure + 4.0);
}

float LkmmhrrruuiKFJD(float3 Color, float Exposure) 
{
	return rcp(Color.g * Exposure + 1.0);
}

float PPPOIIJDJFGQ3G(float Color, float Exposure) 
{
	return rcp(Color * Exposure + 1.0);
}



float PPPOIIJofFJJJS(float Color, float Exposure) 
{
	return rcp(Color * Exposure + 4.0);
}


inline float PPPOIIkfjSSD(float3 Color, float Exposure) 
{
	return 4.0 * rcp(LLfsdg(Color) * (-Exposure) + 1.0);
}

float PPPOFODJDHSSDf(float3 Color, float Exposure) 
{
	return rcp(Color.g * (-Exposure) + 1.0);
}



float PPPOFOdfgrr5433gf(float Color, float Exposure) 
{
	return rcp(Color * (-Exposure) + 1.0);
}





float PPPOFOddfrruief4g(float Channel, float Exposure) 
{
	return Channel * PPPOFOdfgrr5433gf(Channel, Exposure);
}


float PPpyetejshgueEG(float3 Color, float Exposure) 
{
	float L = LLfsdg(Color);
	return L * PPPOIIJofFJJJS(L, Exposure);
}

float PPpyeporueuSDJHHF(float3 Color, float Exposure) 
{
	return Color.g * PPPOIIJDJFGQ3G(Color.g, Exposure);
}



float PPpyepfporjDIFHHF(float Channel) 
{
	return Channel * rcp(1.0 + Channel);
}
	
float HighlightDecompression(float Channel) 
{
	return Channel * rcp(1.0 - Channel);
}

float KFJDIEHdjhdjh4djhsfjsg(float3 Color, float Exposure) 
{
	return sqrt(PPpyepfporjDIFHHF(Lkmmmdr46dFFFFdeq(Color) * Exposure));
}

float KdfgKFKLDJRDF(float Channel) 
{
	
	return HighlightDecompression(Channel * Channel);
}


inline float KOPODFJjfhj44hsDHSJHDHJF(float3 Dir, float3 Org, float3 Box)
{
	
	float3 RcpDir = rcp(Dir);
	float3 TNeg = (  Box  - Org) * RcpDir;
	float3 TPos = ((-Box) - Org) * RcpDir;
	return max(max(min(TNeg.x, TPos.x), min(TNeg.y, TPos.y)), min(TNeg.z, TPos.z));
}



inline float KOFDOPJFHfhjf4FHDHDJ(float3 History, float3 dffgdffjkdhfjugSD, float3 fdgrDPOSJSDds, float3 thFJDFgSJSDds, float tfdoirhrRRRdfsrRER)
{
	float3 Min = min(dffgdffjkdhfjugSD, min(fdgrDPOSJSDds, thFJDFgSJSDds));
	float3 Max = max(dffgdffjkdhfjugSD, max(fdgrDPOSJSDds, thFJDFgSJSDds));	

	float3 Avg2 = Max + Min;
	
	float3 Dir = dffgdffjkdhfjugSD - History;
	float3 Org = History - Avg2 * 0.5;
	float3 Scale = Max - Avg2 * tfdoirhrRRRdfsrRER;
	return saturate(KOPODFJjfhj44hsDHSJHDHJF(Dir, Org, Scale));	
}

float KOFDgOOOOODhdh4(float3 Color, float Exposure) 
{
	return rcp(max(Lkmmmdr46dFFFFdeq(Color) * Exposure, 1.0));
}

float4 HdrLerp(float4 ColorA, float4 ColorB, float Blend, float Exposure) 
{
	float BlendA = (1.0 - Blend) * KOFDgOOOOODhdh4(ColorA.rgb, Exposure);
	float BlendB =        Blend  * KOFDgOOOOODhdh4(ColorB.rgb, Exposure);
	float RcpBlend = rcp(BlendA + BlendB);
	BlendA *= RcpBlend;
	BlendB *= RcpBlend;
	return ColorA * BlendA + ColorB * BlendB;
}



struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = v.texcoord.xy;

	/*
	#if UNITY_UV_STARTS_AT_TOP
		o.uv = v.texcoord.xy;
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif		
		*/

	return o;
}

float4 frag (v2f i) : COLOR
{


 // GET MOTION DATA
 // ------------------------------------------------

 float2 KOFUIFHHFHhfjdhHDFHJG345;
 float2 KOFUISHSHJHEHHS;
		
 float  KOFr454dJDFKHDHHFSFP4SG = 1;
  
 float2  urhDFgh4DGHSH44DGD = _MainTex_TexelSize.xy;

 

 //###################################################

 float urhdfgDFIFJR4F = 1-Linear01Depth(tex2D (_CameraDepthTexture, i.uv).x);
 
 float CROSS_SIZE = 1;
 float2 xcdRGRE[4];
 
 xcdRGRE[0] = float2( -urhDFgh4DGHSH44DGD.x, -urhDFgh4DGHSH44DGD.y )*CROSS_SIZE;
 xcdRGRE[1] = float2(  urhDFgh4DGHSH44DGD.x, -urhDFgh4DGHSH44DGD.y )*CROSS_SIZE;
 xcdRGRE[2] = float2( -urhDFgh4DGHSH44DGD.x,  urhDFgh4DGHSH44DGD.y )*CROSS_SIZE;
 xcdRGRE[3] = float2(  urhDFgh4DGHSH44DGD.x,  urhDFgh4DGHSH44DGD.y )*CROSS_SIZE;
 
 float Depths[4];
 Depths[0] = 1-Linear01Depth(tex2D (_CameraDepthTexture, i.uv + xcdRGRE[0] ).x);
 Depths[1] = 1-Linear01Depth(tex2D (_CameraDepthTexture, i.uv + xcdRGRE[1] ).x);
 Depths[2] = 1-Linear01Depth(tex2D (_CameraDepthTexture, i.uv + xcdRGRE[2] ).x);
 Depths[3] = 1-Linear01Depth(tex2D (_CameraDepthTexture, i.uv + xcdRGRE[3] ).x);
 
 int dIndx0;
 if(Depths[0] > Depths[1]) dIndx0 = 0;
 else dIndx0 = 1;
 
 int dIndx1;
 if(Depths[2] > Depths[3]) dIndx1 = 2;
 else dIndx1 = 3;
 
 int dIndx2;
 if(Depths[dIndx0] > Depths[dIndx1]) dIndx2 = dIndx0;
 else dIndx2 = dIndx1;
 
 //-----------------------------------
 int dIndx0C;
 if(Depths[0] < Depths[1]) dIndx0C = 0;
 else dIndx0C = 1;
 
 int dIndx1C;
 if(Depths[2] < Depths[3]) dIndx1C = 2;
 else dIndx1C = 3;
 
 int dIndx2C;
 if(Depths[dIndx0C] < Depths[dIndx1C]) dIndx2C = dIndx0C;
 else dIndx2C = dIndx1C;
 
 //-----------------------------------

 float2 RPFJJFDK44JD = float2(0,0);
 
 if( Depths[dIndx2] > urhdfgDFIFJR4F)
 {
 	RPFJJFDK44JD = xcdRGRE[dIndx2];
 }


 //###################################################

 //Use Motion Vectors Unity
 half2 RPJJSDIiRRHF = tex2D(_CameraMotionVectorsTexture, i.uv + RPFJJFDK44JD).rg;

 KOFUIFHHFHhfjdhHDFHJG345 =   RPJJSDIiRRHF;

 //###################################################


 float RPJJOPRDJDWD = 2;
 float RPJJOSDJSDHEIHSHHS = saturate(abs(KOFUIFHHFHhfjdhHDFHJG345.x) * RPJJOPRDJDWD + abs(KOFUIFHHFHhfjdhHDFHJG345.y) * RPJJOPRDJDWD);
 	
	half2  uv = i.uv ;

	
					
	half4 RPJJOSDJSDyrOR = tex2D(_MainTex, uv.xy - urhDFgh4DGHSH44DGD );
	half4 RPJJSJDSHEIE = tex2D(_MainTex, uv.xy + float2(  0, -urhDFgh4DGHSH44DGD.y ) );
	half4 RPJJSPSDODSJSeV = tex2D(_MainTex, uv.xy + float2(  urhDFgh4DGHSH44DGD.x, -urhDFgh4DGHSH44DGD.y ) );
	half4 RPJrgrrDFGV = tex2D(_MainTex, uv.xy + float2(  -urhDFgh4DGHSH44DGD.x, 0 ) );
	half4 RPPORJHSDHJF = tex2D(_MainTex, uv.xy);
	half4 RPPKSDJSDKERRS = tex2D(_MainTex, uv.xy + float2(   urhDFgh4DGHSH44DGD.x, 0 ) );
	half4 RPPfgPSDHeHDS = tex2D(_MainTex, uv.xy + float2( -urhDFgh4DGHSH44DGD.x,  urhDFgh4DGHSH44DGD.y ) );
	half4 RPPfgPSfgEERR = tex2D(_MainTex, uv.xy + float2(  0,  urhDFgh4DGHSH44DGD.y ) );
	half4 RPPfdfGGR = tex2D(_MainTex, uv.xy + urhDFgh4DGHSH44DGD );


		

        half RdfgGDDDDGGd = (Lkmmmdr46dFFFFdeq(tex2D(_MainTex, uv.xy +  float2(  0, -urhDFgh4DGHSH44DGD.y  ) )) );
        half RdfgGfdgdfrrFD = (Lkmmmdr46dFFFFdeq( tex2D(_MainTex, uv.xy + float2(  0,  urhDFgh4DGHSH44DGD.y  ) )) );
        half RdfgDFJFDJ = (Lkmmmdr46dFFFFdeq(tex2D(_MainTex, uv.xy  + float2(  urhDFgh4DGHSH44DGD.x , 0  ) )) );
        half FDODFJfd = (Lkmmmdr46dFFFFdeq(tex2D(_MainTex, uv.xy  + float2( -urhDFgh4DGHSH44DGD.x , 0  ) )) );


        half FDODFJfdDGr = (Lkmmmdr46dFFFFdeq(tex2D(_MainTex, uv.xy))) - (RdfgGDDDDGGd + RdfgGfdgdfrrFD + RdfgDFJFDJ + FDODFJfd)*0.25;
//adklfie IIUJDLA jfaleije DIUEUUKK fjjafeiIIUIj quoewiuOU EUAOI DJFOUJLolkjdfowei aouiioqwuoiuoiuioukdjivcj
 //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDOdfg NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI
 //IUDIUFUFIFA JIOI JMOAIEFJ MAIJAFPIO  A JFPEIOJF AMFAOEIFJAF ANFB AFUIWEFNA FHNAIEU NAFEIFLAL FN AFIHAIEIFBN
 //EYUIAY IHIFAU EN  NFAIUENFN NAFIEUHIU NOAfdhhIUFHWEKJNA N FANLEFUHALNF ADKFNFAIUH NFAIKHA NFANIAFNKJN NIAEKKEUR
  //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI 

        half FD4DF5FD6DFGHHDDGHS4DGH       = saturate(abs(FDODFJfdDGr));
        FD4DF5FD6DFGHHDDGHS4DGH = saturate(pow(FD4DF5FD6DFGHHDDGHS4DGH, 4.0)*0.5);
        KOFr454dJDFKHDHHFSFP4SG = lerp(2.5,12,FD4DF5FD6DFGHHDDGHS4DGH)*(0.1*_ControlParams.w);        		
		RPJJOSDJSDyrOR.rgb *= Lkmmmfggfdrtgte(RPJJOSDJSDyrOR.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPJJSJDSHEIE.rgb *= Lkmmmfggfdrtgte(RPJJSJDSHEIE.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPJJSPSDODSJSeV.rgb *= Lkmmmfggfdrtgte(RPJJSPSDODSJSeV.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPJrgrrDFGV.rgb *= Lkmmmfggfdrtgte(RPJrgrrDFGV.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPPORJHSDHJF.rgb *= Lkmmmfggfdrtgte(RPPORJHSDHJF.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPPKSDJSDKERRS.rgb *= Lkmmmfggfdrtgte(RPPKSDJSDKERRS.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPPfgPSDHeHDS.rgb *= Lkmmmfggfdrtgte(RPPfgPSDHeHDS.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPPfgPSfgEERR.rgb *= Lkmmmfggfdrtgte(RPPfgPSfgEERR.rgb, KOFr454dJDFKHDHHFSFP4SG);
		RPPfdfGGR.rgb *= Lkmmmfggfdrtgte(RPPfdfGGR.rgb, KOFr454dJDFKHDHHFSFP4SG);
		
//adklfie IIUJDLA jfaledfgije DIUEUUKK fjjafeiIIUIj quoewiuOU EUAOI DJFOUJLolkjdfowei aouiioqwuoiuoiuioukdjivcj
 //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI
 //IUDIUFUFIFA JIOI JMOAIEFJ MAIJAFPIO  A JFPEIOJF AMFAOEIFJAF ANFB AFUIWEFNA FHNAIEU NAFEIFLAL FN AFIHAIEIFBN
 //EYUIAY IHIFAU EN  NFAIUENFN NAFIEUHIU NOAIUFHWEKJNA N FANLEFUHALNF ADKFNFAIUH NFAIKdfgHA NFANIAFNKJN NIAEKKEUR
  //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI 
			
		half4 dffgdffjkdhfjugSD= 
			RPJJOSDJSDyrOR * 0.0625 + 
			RPJJSJDSHEIE * 0.125 +
			RPJJSPSDODSJSeV * 0.0625 +
			RPJrgrrDFGV * 0.125 +
			RPPORJHSDHJF * 0.25 +
			RPPKSDJSDKERRS * 0.125 +
			RPPfgPSDHeHDS * 0.0625 +
			RPPfgPSfgEERR * 0.125 +
			RPPfdfGGR * 0.0625;


						
			
		float4	 dffgdffdgrrriyr = dffgdffjkdhfjugSD;
			

		half4 dffgdDFPOFDJDF = min(min(RPJJOSDJSDyrOR, RPJJSPSDODSJSeV), min(RPPfgPSDHeHDS, RPPfdfGGR));		
		half4 dffgSDPOSJSDds = max(max(RPJJOSDJSDyrOR, RPJJSPSDODSJSeV), max(RPPfgPSDHeHDS, RPPfdfGGR));		
		half4 fdgrDPOSJSDds = min(min(min(RPJJSJDSHEIE, RPJrgrrDFGV), min(RPPORJHSDHJF, RPPKSDJSDKERRS)), RPPfgPSfgEERR);		
		half4 thFJDFgSJSDds = max(max(max(RPJJSJDSHEIE, RPJrgrrDFGV), max(RPPORJHSDHJF, RPPKSDJSDKERRS)), RPPfgPSfgEERR);		
		dffgdDFPOFDJDF = min(dffgdDFPOFDJDF, fdgrDPOSJSDds);
		dffgSDPOSJSDds = max(dffgSDPOSJSDds, thFJDFgSJSDds);
	    fdgrDPOSJSDds = fdgrDPOSJSDds * 0.5 + dffgdDFPOFDJDF * 0.5;
		thFJDFgSJSDds = thFJDFgSJSDds * 0.5 + dffgSDPOSJSDds * 0.5;
		
		
	   //  HISTORY //
	  
	    
		
		float4 thFJDFgSJSDdsdfgDFDF45F = tex2D(_Accum, i.uv-KOFUIFHHFHhfjdhHDFHJG345);

			
			   thFJDFgSJSDdsdfgDFDF45F.rgb *= Lkmmmfggfdrtgte(thFJDFgSJSDdsdfgDFDF45F.rgb, KOFr454dJDFKHDHHFSFP4SG);
		
		
		float thFJDfdiope = LLfsdg(fdgrDPOSJSDds.rgb);
		float thFPRRJDF = LLfsdg(thFJDFgSJSDds.rgb);
		float throridRRRD = LLfsdg(thFJDFgSJSDdsdfgDFDF45F.rgb);

		float tfdoirhrRRR = thFPRRJDF - thFJDfdiope;

				float2	tfdoirhrRRRdfsrRER = lerp( float2(_delValues.x, _delValues.y), float2(_delValues.z, _delValues.w), saturate(length(KOFUIFHHFHhfjdhHDFHJG345)*1000000) );
			

				if(_AntiShimmer < 0.5)
				{
				 tfdoirhrRRRdfsrRER = float2(0.5, 1.0);
				}

			
		_ControlParams.y = _ControlParams.y * tfdoirhrRRRdfsrRER.y ;

		float tfdoirhrRRRdfsrdsdt = KOFDOPJFHfhjf4FHDHDJ(thFJDFgSJSDdsdfgDFDF45F.rgb, dffgdffdgrrriyr.rgb, fdgrDPOSJSDds.rgb, thFJDFgSJSDds.rgb, tfdoirhrRRRdfsrRER.x);

		//adklfie IIUJDLA jfaleije DIUEUUKK fjjafeiIIUIj quoewiuOU EUAOI DJFOUJLolkjdfowei aouiioqwuoiuoiuioukdjivcj
 //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI
 //IUDIUFUFIFA JIOI JMOAIEFJ MAIJAFPIO  A JFPEIOJF AMFAOEIFJAF ANFB AFUIWEFNA FHNAIEU NAFEIFLAL FN AFIHAIEIFBN
 //EYUIAY IHIFAU EN  NFAIUENFN NAFIEUHIU NOAIUFHWEKJNA N FANLEFUHALNF ADKFNFAIUH NFAIKHA NFANIAFNKJN NIAEKKEUR
  //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI

		thFJDFgSJSDdsdfgDFDF45F.rgb = lerp(thFJDFgSJSDdsdfgDFDF45F.rgb, dffgdffdgrrriyr.rgb, tfdoirhrRRRdfsrdsdt );
		  //adklfie IIUJDLA jfaleije DIUEUUKK fjjafeiIIUIj quoewiuOU EUAOI DJFOUJLolkjdfowei aouiioqwuoiuoiuioukdjivcj
 //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI
 //IUDIUFUFIFA JIOI JMOAIEFJ MAIJAFPIO  A JFPEIOJF AMFAOEIFJAF ANFB AFUIWEFNA FHNAIEU NAFEIFLAL FN AFIHAIEIFBN
 //EYUIAY IHIFAU EN  NFAIUENFN NAFIEUHIU NOAIUFHWEKJNA N FANLEFUHALNF ADKFNFAIUH NFAIKHA NFANIAFNKJN NIAEKKEUR
  //qoiuadf mnfneaoiuedkdDLKK PPDLK FJPPL FBDFLUAEIONADFKASNDO NFAOEFUAIFUAEIFNDKFMNFLF NFOIEOIUNFAKLDN JNFOEI 

		float ssfSDPOSJdjjhJD = saturate(RPJJOSDJSDHEIHSHHS) * 0.5;
		float POFDJSdsfsggrPOEJdf =  _ControlParams.z;		
		ssfSDPOSJdjjhJD = saturate(ssfSDPOSJdjjhJD + rcp(1.0 + tfdoirhrRRR * POFDJSdsfsggrPOEJdf));
		dffgdffjkdhfjugSD.rgb = lerp(dffgdffjkdhfjugSD.rgb, RPPORJHSDHJF.rgb, ssfSDPOSJdjjhJD);
		float ssfSDPOSJdjjhJDfgdd = (1.0/_ControlParams.y) + RPJJOSDJSDHEIHSHHS * (1.0/_ControlParams.y);
		float ssfSDPOSJdjjhJdfgdfr4df = throridRRRD * ssfSDPOSJdjjhJDfgdd * (1.0 + RPJJOSDJSDHEIHSHHS * ssfSDPOSJdjjhJDfgdd * 4.0);
		float ssfSDPOSJdPODFJfdhjdf = saturate(ssfSDPOSJdjjhJdfgdfr4df * rcp(throridRRRD + tfdoirhrRRR));
		float POFDJSJdPODFJfdhjdf = lerp(ssfSDPOSJdPODFJfdhjdf, (sqrt(ssfSDPOSJdPODFJfdhjdf)), saturate(length(KOFUIFHHFHhfjdhHDFHJG345)*_AdaptiveResolve) );
		thFJDFgSJSDdsdfgDFDF45F = lerp(thFJDFgSJSDdsdfgDFDF45F, dffgdffjkdhfjugSD, POFDJSJdPODFJfdhjdf);
		thFJDFgSJSDdsdfgDFDF45F.rgb *= PPPOIIkfjSSD(thFJDFgSJSDdsdfgDFDF45F.rgb, KOFr454dJDFKHDHHFSFP4SG);
		thFJDFgSJSDdsdfgDFDF45F.rgb = -min(-thFJDFgSJSDdsdfgDFDF45F.rgb, 0.0);
	 	return thFJDFgSJSDdsdfgDFDF45F ;
	 		
}
ENDCG
	}
}

Fallback off

}