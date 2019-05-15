//LIVENDA CTAA V3 CINEMATIC TEMPORAL ANTI ALIASING
//Copyright Livenda Labs 2019
//pc-v1.7


using UnityEngine;
using System.Collections;




[RequireComponent (typeof(Camera))]
[AddComponentMenu("Image Effects/LIVENDA/CTAA_PC")]
public class CTAA_PC : MonoBehaviour {

	//--------------------------------------------------------------

	[Space(5)]
	public bool CTAA_Enabled 			= true;
	[Header("CTAA Settings")]
	[Range(3, 16)] public int TemporalStability     	   = 8;
	[Space(5)]
	[Range(0.001f, 4.0f)] public float HdrResponse   = 2.0f;
	[Space(5)]
	[Range(0.0f, 10.0f)] public float Sharpness   = 9.52f;
	[Space(5)]
	[Range(0.2f, 0.5f)] public float AdaptiveSharpness       = 0.34f;
	[Space(5)]
	[Range(0.0f, 0.5f)] public float TemporalJitterScale   = 0.5f;
	[Space(5)]
	[Range(0.01f, 10.0f)] public float MicroShimmerReduction   = 3.0f;
	[Space(5)]
	[Range(0.0f, 1.0f)] public float StaticStabilityPower   = 0.51f;
    [Space(5)]
    [Tooltip("Eliminates Micro Shimmer - (No Dynamic Objects) Suitable for Architectural Visualisation, CAD, Engineering or non-moving objects. Camera can be moved.")]
    public bool AntiShimmerMode = true;

    //[Space(10)]
    private Vector4 delValues = new Vector4(0.01f, 2.0f, 0.5f, 0.3f);
    //--------------------------------------------------------------

    private bool PreEnhanceEnabled   	= true;
	private float preEnhanceStrength 	= 1.0f;
	private float preEnhanceClamp    	= 0.005f;
	private float AdaptiveResolve 		= 3000.0f;
	private float jitterScale 			= 1.0f;

	private Material ctaaMat;
	private Material mat_enhance;
	private RenderTexture rtAccum0;
	private RenderTexture rtAccum1;
	private RenderTexture txaaOut;
	private RenderTexture afterPreEnhace;
	private bool firstFrame;
	private bool swap;
	private int frameCounter;
    private Vector3 camoldpos;
    


    private float[] x_jit = new float[] { 0.5f, -0.25f, 0.75f, -0.125f, 0.625f, 0.575f, -0.875f, 0.0625f, -0.3f, 0.75f, -0.25f, -0.625f, 0.325f, 0.975f, -0.075f, 0.625f };
	private float[] y_jit = new float[] { 0.33f, -0.66f, 0.51f, 0.44f, -0.77f, 0.12f, -0.55f, 0.88f, -0.83f, 0.14f, 0.71f, -0.34f, 0.87f, -0.12f, 0.75f, 0.08f };



	void SetCTAA_Parameters()
	{

		PreEnhanceEnabled   	= AdaptiveSharpness > 0.01 ? true : false;
		preEnhanceStrength 		= Mathf.Lerp (0.2f, 2.0f, AdaptiveSharpness);
		preEnhanceClamp    	    = Mathf.Lerp (0.005f, 0.12f, AdaptiveSharpness); 
		jitterScale 			= TemporalJitterScale;
		AdaptiveResolve         = 3000.0f;

        ctaaMat.SetFloat("_AntiShimmer", (AntiShimmerMode ? 1.0f : 0.0f));

        
        ctaaMat.SetVector("_delValues", delValues);

	}

	private static Material CreateMaterial(string shadername)
	{
		if (string.IsNullOrEmpty(shadername)) 
		{
			return null;
		}
		Material material = new Material(Shader.Find(shadername));
		material.hideFlags = HideFlags.HideAndDontSave;
		return material;
	}

	private static void DestroyMaterial(Material mat)
	{
		if (mat != null)
		{
			Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	void Awake ()
	{
		if (ctaaMat == null) 		ctaaMat 		= CreateMaterial("Hidden/CTAA_PC");
		if (mat_enhance == null)   	mat_enhance  	= CreateMaterial("Hidden/CTAA_Enhance_PC");
	
		firstFrame   = true;
		swap	     = true;
		frameCounter = 0;

		SetCTAA_Parameters ();
	}


	void OnEnable () 
	{
		if (ctaaMat == null) 		ctaaMat 		= CreateMaterial("Hidden/CTAA_PC");
		if (mat_enhance == null)   	mat_enhance  	= CreateMaterial("Hidden/CTAA_Enhance_PC");

		firstFrame   = true;
		swap	     = true;
		frameCounter = 0;

		SetCTAA_Parameters ();
		
		Camera mcam = GetComponent<Camera> ();

		mcam.depthTextureMode |= DepthTextureMode.DepthNormals;
		mcam.depthTextureMode |= DepthTextureMode.Depth;
		mcam.depthTextureMode |= DepthTextureMode.MotionVectors;
	
	}


	private void OnDisable()
	{
		DestroyMaterial(ctaaMat);
		DestroyMaterial(mat_enhance);
		DestroyImmediate(rtAccum0);      	   rtAccum0       		= null;
		DestroyImmediate(rtAccum1);    	 	   rtAccum1       		= null;
		DestroyImmediate(txaaOut);       	   txaaOut        		= null;
		DestroyImmediate(afterPreEnhace);      afterPreEnhace       = null;
	}



	void OnPreCull()
	{
		if (CTAA_Enabled) {
			jitterCam ();
		}
	}


	void jitterCam()
	{

		base.GetComponent<Camera>().ResetWorldToCameraMatrix();     // < ----- Unity 2017
		base.GetComponent<Camera>().ResetProjectionMatrix();        // < ----- Unity 2017

		base.GetComponent<Camera>().nonJitteredProjectionMatrix =  base.GetComponent<Camera>().projectionMatrix;

		//base.GetComponent<Camera>().ResetWorldToCameraMatrix();	// < ----- Unity 5.6 
		//base.GetComponent<Camera>().ResetProjectionMatrix();      // < ----- Unity 5.6 
			
			Matrix4x4 matrixx = base.GetComponent<Camera>().projectionMatrix;
			float num  = this.x_jit[this.frameCounter]*jitterScale;
			float num2 = this.y_jit[this.frameCounter]*jitterScale;
			matrixx.m02 += ((num * 2f) - 1f) / base.GetComponent<Camera>().pixelRect.width;
			matrixx.m12 += ((num2 * 2f) - 1f) / base.GetComponent<Camera>().pixelRect.height;
			this.frameCounter++;
			this.frameCounter = this.frameCounter % 16;
			base.GetComponent<Camera>().projectionMatrix = matrixx;

	}



/*
    void Update()
    {


        
        if (transform.hasChanged)
        {          
            transform.hasChanged = false;
            ctaaMat.SetFloat("_CamMotion", 1.0f);
        }
        else
        {
            ctaaMat.SetFloat("_CamMotion", 0.0f);           
        }
       
    }
    */

    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		
		if (CTAA_Enabled) {
			
			SetCTAA_Parameters ();
		
		//--------------------------------------------------------------------------------------------------
		//Create Required RT's
		//--------------------------------------------------------------------------------------------------
		if (((rtAccum0 == null) || (rtAccum0.width != source.width)) || (rtAccum0.height != source.height))
		{
			DestroyImmediate(rtAccum0);
			rtAccum0 = new RenderTexture(source.width, source.height, 0, source.format);
			rtAccum0.hideFlags  = HideFlags.HideAndDontSave;
			rtAccum0.filterMode = FilterMode.Bilinear;
			rtAccum0.wrapMode   = TextureWrapMode.Clamp;

		}

		if (((rtAccum1 == null) || (rtAccum1.width != source.width)) || (rtAccum1.height != source.height))
		{
			DestroyImmediate(rtAccum1);
			rtAccum1 = new RenderTexture(source.width, source.height, 0, source.format);
			rtAccum1.hideFlags  = HideFlags.HideAndDontSave;
			rtAccum1.filterMode = FilterMode.Bilinear;
			rtAccum1.wrapMode   = TextureWrapMode.Clamp;
		}

		if (((txaaOut == null) || (txaaOut.width != source.width)) || (txaaOut.height != source.height))
		{
			DestroyImmediate(txaaOut);
			txaaOut = new RenderTexture(source.width, source.height, 0, source.format);
			txaaOut.hideFlags  = HideFlags.HideAndDontSave;
			txaaOut.filterMode = FilterMode.Bilinear;
			txaaOut.wrapMode   = TextureWrapMode.Clamp;
		}


		if (((afterPreEnhace == null) || (afterPreEnhace.width != source.width)) || (afterPreEnhace.height != source.height))
		{
			DestroyImmediate(afterPreEnhace);
			afterPreEnhace = new RenderTexture(source.width, source.height, 0, source.format);
			afterPreEnhace.hideFlags  = HideFlags.HideAndDontSave;
			afterPreEnhace.filterMode = source.filterMode;
			afterPreEnhace.wrapMode   = TextureWrapMode.Clamp;
		}

		//-----------------------------------------------------------
		if(PreEnhanceEnabled)
		{
			mat_enhance.SetFloat("_AEXCTAA", 1.0f / (float)Screen.width);
			mat_enhance.SetFloat("_AEYCTAA", 1.0f / (float)Screen.height);
			mat_enhance.SetFloat("_AESCTAA", preEnhanceStrength);
			mat_enhance.SetFloat("_AEMAXCTAA", 	  preEnhanceClamp);
			Graphics.Blit(source, afterPreEnhace, mat_enhance, 1);
		}
		else
		{
			Graphics.Blit(source, afterPreEnhace);
		}
		//-----------------------------------------------------------

		if (firstFrame) {
			Graphics.Blit (afterPreEnhace, rtAccum0);
			firstFrame = false;
		}

		ctaaMat.SetFloat ("_AdaptiveResolve", AdaptiveResolve);
			ctaaMat.SetVector ("_ControlParams", new Vector4 (StaticStabilityPower, (float)TemporalStability, HdrResponse, MicroShimmerReduction));

		if (swap) {
			ctaaMat.SetTexture ("_Accum", rtAccum0);
			Graphics.Blit (afterPreEnhace, rtAccum1, ctaaMat);	
			Graphics.Blit (rtAccum1, txaaOut);
		} else {
			ctaaMat.SetTexture ("_Accum", rtAccum1);
			Graphics.Blit (afterPreEnhace, rtAccum0, ctaaMat);
			Graphics.Blit (rtAccum0, txaaOut);
		}

		Graphics.Blit (txaaOut, destination);

		swap = !swap;

		} else {
			
			jitterScale = 0;

			Graphics.Blit (source, destination);

		}


	}

}
