//===============================================================
//LIVENDA CTAA VR FOR OPENVR - CINEMATIC TEMPORAL ANTI-ALIASING
//VIRTUAL REALITY VERSION 1.7
//Copyright Livenda Labs 2017 - 2019
//===============================================================

using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Camera))]
[RequireComponent (typeof(CTAAVR_Velocity_OPENVR))]
[AddComponentMenu("Livenda Effects/CTAAVR_VIVE")]
public class CTAAVR_VIVE : MonoBehaviour {

	public bool CTAA_Enabled 			= true;

	[Header("CTAA Settings")]

    [Tooltip("Bias edges during Anti-Aliasing")]
    [Range(1.0f, 4.0f)] public float TemporalEdgePower    	   = 3.0f;
	[Space(5)]
    [Tooltip("Adjust Camera Jitter Size, Higher Provides Better Anti-Aliasing but less Sharp")]
    [Range(0.0f, 0.5f)] public float TemporalJitterScale   = 0.25f;
	[Space(5)]
	public Camera.StereoscopicEye VRCameraEYE;

	private float TemporalQuality 	 	= 3.0f;

    
    private float jitterScale 			= 0.25f;
	private int forwardMode;

    private RenderTexture afterPreEnhace;
    [Space(5)]
    [Tooltip("Enable to Apply Adaptive Sharpening Filter, small performance hit")]
    public bool SharpnessEnabled = true;
    [Space(5)]
    [Range(0.0f, 1.0f)] public float AdaptiveSharpness = 0.33f;
    private float preEnhanceStrength = 1.0f;
    private float preEnhanceClamp = 0.005f;

    private CTAAVR_Velocity_OPENVR _velocity;

	void Awake()
	{		
		_velocity = GetComponent<CTAAVR_Velocity_OPENVR>();
	}



	private Material mat_txaa;
	private Material mat_enhance;
	private RenderTexture rtAccum0;
	private RenderTexture rtAccum1;
	private RenderTexture txaaOut;

	private bool firstFrame;
	private bool swap;

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

	private void OnEnable()
	{
		firstFrame   = true;
		swap	     = true;

		CreateMaterials();

		Camera _camera = base.GetComponent<Camera> ();
		if (_camera.actualRenderingPath == RenderingPath.Forward) {
			forwardMode = 1;
		} else {
			forwardMode = 0;
		}

	}

	private void OnDisable()
	{		
		DestroyImmediate(rtAccum0);      	   rtAccum0       		= null;
		DestroyImmediate(rtAccum1);    	 	   rtAccum1       		= null;
		DestroyImmediate(txaaOut);       	   txaaOut        		= null;
		DestroyMaterial(mat_txaa);
		DestroyMaterial(mat_enhance);
	}


	private void CreateMaterials()
	{
		if (mat_txaa == null)      	  	  mat_txaa     	  	  = CreateMaterial("Hidden/CTAAVR_VIVE");
		if (mat_enhance == null)   	  	  mat_enhance  	  	  = CreateMaterial("Hidden/AdaptiveEnhanceVR_VIVE");
	}

	void SetCTAA_Parameters()
	{		
		TemporalQuality = TemporalEdgePower;
		jitterScale 	= TemporalJitterScale;

        preEnhanceStrength = Mathf.Lerp(0.2f, 1.5f, AdaptiveSharpness);
        preEnhanceClamp    = Mathf.Lerp(0.005f, 0.008f, AdaptiveSharpness);

    }
	
	void Start () 
	{		
		CreateMaterials();

		Camera _camera = base.GetComponent<Camera> ();
		if (_camera.actualRenderingPath == RenderingPath.Forward) {
			forwardMode = 1;
		} else {
			forwardMode = 0;
		}

		_camera.depthTextureMode = DepthTextureMode.Depth;

		SetCTAA_Parameters ();

		StartCoroutine (fixCam ());

	}

	IEnumerator fixCam()
	{
		Camera _camera = base.GetComponent<Camera> ();

		if (_camera.actualRenderingPath == RenderingPath.Forward) {

			_camera.renderingPath = RenderingPath.DeferredShading;

			yield return new WaitForSeconds (0.5f);

			_camera.renderingPath = RenderingPath.Forward;
		} 

		yield return new WaitForSeconds (0.1f);
	}


	private int frameCounter;


	private float[] x_jit = new float[] { 0.5f, -0.25f, 0.75f, -0.125f, 0.625f, 0.575f, -0.875f, 0.0625f, -0.3f, 0.75f, -0.25f, -0.625f, 0.325f, 0.975f, -0.075f, 0.625f };
	private float[] y_jit = new float[] { 0.33f, -0.66f, 0.51f, 0.44f, -0.77f, 0.12f, -0.55f, 0.88f, -0.83f, 0.14f, 0.71f, -0.34f, 0.87f, -0.12f, 0.75f, 0.08f };

	void OnPreCull()
	{
		jitterCam ();
	}


	void jitterCam()
	{
		
		Camera _camera = base.GetComponent<Camera> ();
		base.GetComponent<Camera>().ResetStereoProjectionMatrices();
		Matrix4x4 matrixx = _camera.GetStereoProjectionMatrix(VRCameraEYE);
		float num  = this.x_jit[this.frameCounter]*jitterScale;
		float num2 = this.y_jit[this.frameCounter]*jitterScale;
		matrixx.m02 += ((num * 2f) - 1f) / base.GetComponent<Camera>().pixelRect.width;
		matrixx.m12 += ((num2 * 2f) - 1f) / base.GetComponent<Camera>().pixelRect.height;
		this.frameCounter++;
		this.frameCounter = this.frameCounter % 16;

		_camera.SetStereoProjectionMatrix(VRCameraEYE, matrixx);

	}


	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		
		SetCTAA_Parameters ();
				
		CreateMaterials();


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
                afterPreEnhace.hideFlags = HideFlags.HideAndDontSave;
                afterPreEnhace.filterMode = source.filterMode;
                afterPreEnhace.wrapMode = TextureWrapMode.Clamp;
            }

            _velocity.RenderVel();


			if(CTAA_Enabled)
			{

                if (SharpnessEnabled)
                {
                    mat_enhance.SetFloat("_DELTAXp", 1.0f / (float)Screen.width);
                    mat_enhance.SetFloat("_DELTAYp", 1.0f / (float)Screen.height);
                    mat_enhance.SetFloat("_Strength", preEnhanceStrength);
                    mat_enhance.SetFloat("_DELTAMAXC", preEnhanceClamp);

                    Graphics.Blit(source, afterPreEnhace, mat_enhance, 1);

                    //======================================================================
                    mat_txaa.SetFloat("_RenderPath", (float)forwardMode);

                    if (firstFrame)
                    {
                        Graphics.Blit(afterPreEnhace, rtAccum0);
                        firstFrame = false;
                    }

                    mat_txaa.SetTexture("_Motion0", _velocity.velocityBuffer);

                    float tempqual = (float)TemporalQuality;
                    mat_txaa.SetVector("_ControlParams", new Vector4(0, tempqual, 0, 0));

                    if (swap)
                    {
                        mat_txaa.SetTexture("_Accum", rtAccum0);
                        Graphics.Blit(afterPreEnhace, rtAccum1, mat_txaa);
                        Graphics.Blit(rtAccum1, destination);
                    }
                    else
                    {
                        mat_txaa.SetTexture("_Accum", rtAccum1);
                        Graphics.Blit(afterPreEnhace, rtAccum0, mat_txaa);
                        Graphics.Blit(rtAccum0, destination);
                    }
                    //======================================================================

                }
                else
                {
                    //======================================================================
                    mat_txaa.SetFloat("_RenderPath", (float)forwardMode);

                    if (firstFrame)
                    {
                        Graphics.Blit(source, rtAccum0);
                        firstFrame = false;
                    }

                    mat_txaa.SetTexture("_Motion0", _velocity.velocityBuffer);

                    float tempqual = (float)TemporalQuality;
                    mat_txaa.SetVector("_ControlParams", new Vector4(0, tempqual, 0, 0));

                    if (swap)
                    {
                        mat_txaa.SetTexture("_Accum", rtAccum0);
                        Graphics.Blit(source, rtAccum1, mat_txaa);
                        Graphics.Blit(rtAccum1, destination);
                    }
                    else
                    {
                        mat_txaa.SetTexture("_Accum", rtAccum1);
                        Graphics.Blit(source, rtAccum0, mat_txaa);
                        Graphics.Blit(rtAccum0, destination);
                    }
                    //======================================================================

                }






            }
            else
			{
			    Graphics.Blit(source, destination);
			}
		

			swap = !swap;	

	}

}
