//===============================================================
//LIVENDA CTAA VR FOR OPENVR - CINEMATIC TEMPORAL ANTI-ALIASING
//VIRTUAL REALITY VERSION
//Copyright Livenda Labs 2017 - 2019
//Single Pass Stereo Render Mode Support V1.7
//===============================================================
#if UNITY_5_5_OR_NEWER
#define SUPPORT_STEREO
#endif

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Livenda Effects/CTAAVR_SPS")]
public class CTAAVR_SPS : MonoBehaviour
{
    #if UNITY_PS4
        private const RenderTextureFormat velocityFormat = RenderTextureFormat.RGHalf;
    #else
        private const RenderTextureFormat velocityFormat = RenderTextureFormat.RGFloat;
    #endif

    public bool CTAA_Enabled = true;

    [Header("CTAA Settings")]

    [Tooltip("Bias edges during Anti-Aliasing")]
    [Range(1.0f, 4.0f)] public float TemporalEdgePower = 1.0f;
    [Space(5)]
    [Tooltip("Enable to Apply Adaptive Sharpening Filter, small performance hit")]
    public bool SharpnessEnabled = true;
    [Space(5)]
    [Range(0.0f, 0.5f)] public float AdaptiveSharpness = 0.1f;


    //Internal Use
    private float preEnhanceStrength = 1.0f;
    private float preEnhanceClamp = 0.005f;
    private float TemporalQuality = 1.0f;
    private int forwardMode;    
    private Material mat_txaa;  
    private Material StereoMat;
    private Material mat_enhance;
    private RenderTexture rtAccum0;
    private RenderTexture rtAccum1;
    private RenderTexture txaaOut;
    private RenderTexture afterPreEnhace;
    private Camera _camera;
    private bool firstFrame;
    private bool swap;      
    private Shader velShader;
    private Material velocityMaterial;
    private RenderTexture[] velBuffers;
    private bool[] paramInitialized;
    private Vector4[] paramProjectionExtents;
    private Matrix4x4[] paramCurrV;
    private Matrix4x4[] paramCurrVP;
    private Matrix4x4[] paramPrevVP;
    private Matrix4x4[] paramPrevVP_NoFlip;
    private int activeEyeIndex = -1;
    public RenderTexture getCurrentVelocityBuffer { get { return (activeEyeIndex != -1) ? velBuffers[activeEyeIndex] : null; } }
    
    void Awake()
    {        
        _camera = GetComponent<Camera>();
        velShader = Shader.Find("Hidden/VelocityShaderCTAASPS");
        if (mat_enhance == null) mat_enhance = CreateMaterial("Hidden/CTAA_Enhance_SPS");

        Clear();
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
     
    private void OnEnable()
    {
        if (mat_enhance == null) mat_enhance = CreateMaterial("Hidden/CTAA_Enhance_SPS");

        firstFrame = true;
        swap = true;

        CreateMaterials();

        if(_camera == null) _camera = base.GetComponent<Camera>();

        if (_camera.actualRenderingPath == RenderingPath.Forward)
        {
            forwardMode = 1;
        }
        else
        {
            forwardMode = 0;
        }
    }

    private void OnDisable()
    {
        DestroyImmediate(rtAccum0); rtAccum0 = null;
        DestroyImmediate(rtAccum1); rtAccum1 = null;
        DestroyImmediate(txaaOut); txaaOut = null;
        DestroyMaterial(mat_txaa);      
        DestroyMaterial(StereoMat);
        DestroyMaterial(mat_enhance);
    }

    private void CreateMaterials()
    {
        if (mat_txaa == null) mat_txaa = CreateMaterial("Hidden/CTAA301_SPS_SHADER");      
        if (StereoMat == null) StereoMat = CreateMaterial("Hidden/CTAA_Stereo");
    }

    void SetCTAA_Parameters()
    {
        TemporalQuality = TemporalEdgePower;

        preEnhanceStrength = Mathf.Lerp(0.2f, 2.0f, AdaptiveSharpness);
        preEnhanceClamp    = Mathf.Lerp(0.005f, 0.12f, AdaptiveSharpness);

    }

    void Start()
    {
        if (velShader == null) velShader = Shader.Find("Hidden/VelocityShaderCTAASPS");
      

        CreateMaterials();
        
        if (_camera.actualRenderingPath == RenderingPath.Forward)
        {
            forwardMode = 1;
        }
        else
        {
            forwardMode = 0;
        }

        _camera.depthTextureMode = DepthTextureMode.Depth;

        SetCTAA_Parameters();

    }

    private int frameCounter;

    void OnPreRender()
    {
        EnsureDepthTexture(_camera);
    }

    void OnPostRender()
    {
        EnsureArray(ref velBuffers, 2);
        EnsureArray(ref paramInitialized, 2, initialValue: false);
        EnsureArray(ref paramProjectionExtents, 2);
        EnsureArray(ref paramCurrV, 2);
        EnsureArray(ref paramCurrVP, 2);
        EnsureArray(ref paramPrevVP, 2);
        EnsureArray(ref paramPrevVP_NoFlip, 2);
        EnsureMaterial(ref velocityMaterial, velShader);
        if (velocityMaterial == null)
            return;


#if SUPPORT_STEREO
        int eyeIndex = (_camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) ? 1 : 0;
#else
        int eyeIndex = 0;
#endif
        int bufferW = _camera.pixelWidth;
        int bufferH = _camera.pixelHeight;

        if (EnsureRenderTarget(ref velBuffers[eyeIndex], bufferW, bufferH, velocityFormat, FilterMode.Point, depthBits: 16))
            Clear();

        EnsureKeyword(velocityMaterial, "CAMERA_PERSPECTIVE", !_camera.orthographic);
        EnsureKeyword(velocityMaterial, "CAMERA_ORTHOGRAPHIC", _camera.orthographic);

#if SUPPORT_STEREO
        if (_camera.stereoEnabled)
        {
            for (int i = 0; i != 2; i++)
            {
                Camera.StereoscopicEye eye = (Camera.StereoscopicEye)i;

                Matrix4x4 currV = _camera.GetStereoViewMatrix(eye);
                Matrix4x4 currP = GL.GetGPUProjectionMatrix(_camera.GetStereoProjectionMatrix(eye), true);
                Matrix4x4 currP_NoFlip = GL.GetGPUProjectionMatrix(_camera.GetStereoProjectionMatrix(eye), false);
                Matrix4x4 prevV = paramInitialized[i] ? paramCurrV[i] : currV;

                paramInitialized[i] = true;
                paramProjectionExtents[i] = GetProjectionExtentsCTAA(_camera, eye);// _camera.GetProjectionExtents(eye);
                paramCurrV[i] = currV;
                paramCurrVP[i] = currP * currV;
                paramPrevVP[i] = currP * prevV;
                paramPrevVP_NoFlip[i] = currP_NoFlip * prevV;
            }

        }
#endif

        RenderTexture activeRT = RenderTexture.active;
        RenderTexture.active = velBuffers[eyeIndex];
        {
            GL.Clear(true, true, Color.black);

            const int kPrepass = 0;
            const int kVertices = 1;
            const int kVerticesSkinned = 2;

#if SUPPORT_STEREO
            velocityMaterial.SetVectorArray("_ProjectionExtents", paramProjectionExtents);
            velocityMaterial.SetMatrixArray("_CurrV", paramCurrV);
            velocityMaterial.SetMatrixArray("_CurrVP", paramCurrVP);
            velocityMaterial.SetMatrixArray("_PrevVP", paramPrevVP);
            velocityMaterial.SetMatrixArray("_PrevVP_NoFlip", paramPrevVP_NoFlip);
#else
            velocityMaterial.SetVector("_ProjectionExtents", paramProjectionExtents[0]);
            velocityMaterial.SetMatrix("_CurrV", paramCurrV[0]);
            velocityMaterial.SetMatrix("_CurrVP", paramCurrVP[0]);
            velocityMaterial.SetMatrix("_PrevVP", paramPrevVP[0]);
            velocityMaterial.SetMatrix("_PrevVP_NoFlip", paramPrevVP_NoFlip[0]);
#endif
            velocityMaterial.SetPass(kPrepass);
            DrawFullscreenQuad();
            var obs = DynamicObjectCTAASPS.activeObjects;

            for (int i = 0, n = obs.Count; i != n; i++)
            {
                var ob = obs[i];
                if (ob != null && ob.rendering && ob.mesh != null)
                {
                    velocityMaterial.SetMatrix("_CurrM", ob.localToWorldCurr);
                    velocityMaterial.SetMatrix("_PrevM", ob.localToWorldPrev);
                    velocityMaterial.SetPass(ob.meshSmrActive ? kVerticesSkinned : kVertices);

                    for (int j = 0; j != ob.mesh.subMeshCount; j++)
                    {
                        Graphics.DrawMeshNow(ob.mesh, Matrix4x4.identity, j);

                    }

                }
            }
        }

        RenderTexture.active = activeRT;
        activeEyeIndex = eyeIndex;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetCTAA_Parameters();

        CreateMaterials();

        if (((rtAccum0 == null) || (rtAccum0.width != source.width)) || (rtAccum0.height != source.height))
        {
            DestroyImmediate(rtAccum0);
            rtAccum0 = new RenderTexture(source.width, source.height, 0, source.format);
            rtAccum0.hideFlags = HideFlags.HideAndDontSave;
            rtAccum0.filterMode = FilterMode.Bilinear;
            rtAccum0.antiAliasing = source.antiAliasing;
            rtAccum0.wrapMode = TextureWrapMode.Clamp;

        }

        if (((rtAccum1 == null) || (rtAccum1.width != source.width)) || (rtAccum1.height != source.height))
        {
            DestroyImmediate(rtAccum1);
            rtAccum1 = new RenderTexture(source.width, source.height, 0, source.format);
            rtAccum1.hideFlags = HideFlags.HideAndDontSave;
            rtAccum1.filterMode = FilterMode.Bilinear;
            rtAccum1.antiAliasing = source.antiAliasing;
            rtAccum1.wrapMode = TextureWrapMode.Clamp;
        }

        if (((txaaOut == null) || (txaaOut.width != source.width)) || (txaaOut.height != source.height))
        {
            DestroyImmediate(txaaOut);
            txaaOut = new RenderTexture(source.width, source.height, 0, source.format);
            txaaOut.hideFlags = HideFlags.HideAndDontSave;
            txaaOut.filterMode = FilterMode.Bilinear;
            txaaOut.antiAliasing = source.antiAliasing;
            txaaOut.wrapMode = TextureWrapMode.Clamp;

        }

        if (((afterPreEnhace == null) || (afterPreEnhace.width != source.width)) || (afterPreEnhace.height != source.height))
        {
            DestroyImmediate(afterPreEnhace);
            afterPreEnhace = new RenderTexture(source.width, source.height, 0, source.format);
            afterPreEnhace.hideFlags = HideFlags.HideAndDontSave;
            afterPreEnhace.filterMode = source.filterMode;
            afterPreEnhace.wrapMode = TextureWrapMode.Clamp;
        }

       
       


        if (CTAA_Enabled)
        {

            if (SharpnessEnabled)
            {
                mat_enhance.SetFloat("_AEXCTAA", 1.0f / (float)Screen.width);
                mat_enhance.SetFloat("_AEYCTAA", 1.0f / (float)Screen.height);
                mat_enhance.SetFloat("_AESCTAA", preEnhanceStrength);
                mat_enhance.SetFloat("_AEMAXCTAA", preEnhanceClamp);
                Graphics.Blit(source, afterPreEnhace, mat_enhance, 1);

                //==================================================
                mat_txaa.SetFloat("_RenderPath", (float)forwardMode);

                if (firstFrame)
                {
                    Graphics.Blit(afterPreEnhace, rtAccum0);
                    firstFrame = false;
                }

                mat_txaa.SetTexture("_Motion0", getCurrentVelocityBuffer);

                float tempqual = (float)TemporalQuality;
                mat_txaa.SetVector("_ControlParams", new Vector4(0, tempqual, 0, 0));

                if (swap)
                {
                    mat_txaa.SetTexture("_Accum", rtAccum0);
                    Graphics.Blit(afterPreEnhace, rtAccum1, mat_txaa);
                    Graphics.Blit(rtAccum1, destination, StereoMat);
                }
                else
                {
                    mat_txaa.SetTexture("_Accum", rtAccum1);
                    Graphics.Blit(afterPreEnhace, rtAccum0, mat_txaa);
                    Graphics.Blit(rtAccum0, destination, StereoMat);
                }
                //==================================================

            }
            else
            {
                //==================================================
                mat_txaa.SetFloat("_RenderPath", (float)forwardMode);

                if (firstFrame)
                {
                    Graphics.Blit(source, rtAccum0);
                    firstFrame = false;
                }

                mat_txaa.SetTexture("_Motion0", getCurrentVelocityBuffer);

                float tempqual = (float)TemporalQuality;
                mat_txaa.SetVector("_ControlParams", new Vector4(0, tempqual, 0, 0));

                if (swap)
                {
                    mat_txaa.SetTexture("_Accum", rtAccum0);
                    Graphics.Blit(source, rtAccum1, mat_txaa);
                    Graphics.Blit(rtAccum1, destination, StereoMat);
                }
                else
                {
                    mat_txaa.SetTexture("_Accum", rtAccum1);
                    Graphics.Blit(source, rtAccum0, mat_txaa);
                    Graphics.Blit(rtAccum0, destination, StereoMat);
                }
                //==================================================
            }



        }
        else
        {
            Graphics.Blit(source, destination);
        }
        
        swap = !swap;

    }

#if SUPPORT_STEREO
    private  Vector4 GetProjectionExtentsCTAA(Camera camera, Camera.StereoscopicEye eye)
    {
        return GetProjectionExtentsCTAA(camera, eye, 0.0f, 0.0f);
    }

    private Vector4 GetProjectionExtentsCTAA(Camera camera, Camera.StereoscopicEye eye, float texelOffsetX, float texelOffsetY)
    {
        Matrix4x4 inv = Matrix4x4.Inverse(camera.GetStereoProjectionMatrix(eye));
        Vector3 ray00 = inv.MultiplyPoint3x4(new Vector3(-1.0f, -1.0f, 0.95f));
        Vector3 ray11 = inv.MultiplyPoint3x4(new Vector3(1.0f, 1.0f, 0.95f));

        ray00 /= -ray00.z;
        ray11 /= -ray11.z;

        float oneExtentX = 0.5f * (ray11.x - ray00.x);
        float oneExtentY = 0.5f * (ray11.y - ray00.y);
        float texelSizeX = oneExtentX / (0.5f * camera.pixelWidth);
        float texelSizeY = oneExtentY / (0.5f * camera.pixelHeight);
        float oneJitterX = 0.5f * (ray11.x + ray00.x) + texelSizeX * texelOffsetX;
        float oneJitterY = 0.5f * (ray11.y + ray00.y) + texelSizeY * texelOffsetY;

        return new Vector4(oneExtentX, oneExtentY, oneJitterX, oneJitterY);
    }
#endif


    void OnApplicationQuit()
    {
        if (velBuffers != null)
        {
            ReleaseRenderTarget(ref velBuffers[0]);
            ReleaseRenderTarget(ref velBuffers[1]);
        }
    }

    void Clear()
    {
        EnsureArray(ref paramInitialized, 2);
        paramInitialized[0] = false;
        paramInitialized[1] = false;
    }

    void EnsureArray<T>(ref T[] array, int size, T initialValue = default(T))
    {
        if (array == null || array.Length != size)
        {
            array = new T[size];
            for (int i = 0; i != size; i++)
                array[i] = initialValue;
        }
    }

    void EnsureArray<T>(ref T[,] array, int size0, int size1, T defaultValue = default(T))
    {
        if (array == null || array.Length != size0 * size1)
        {
            array = new T[size0, size1];
            for (int i = 0; i != size0; i++)
            {
                for (int j = 0; j != size1; j++)
                    array[i, j] = defaultValue;
            }
        }
    }

    void EnsureMaterial(ref Material material, Shader shader)
    {
        if (shader != null)
        {
            if (material == null || material.shader != shader)
                material = new Material(shader);
            if (material != null)
                material.hideFlags = HideFlags.DontSave;
        }
    }

    void EnsureDepthTexture(Camera camera)
    {
        if ((camera.depthTextureMode & DepthTextureMode.Depth) == 0)
            camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    void EnsureKeyword(Material material, string name, bool enabled)
    {
        if (enabled != material.IsKeywordEnabled(name))
        {
            if (enabled)
                material.EnableKeyword(name);
            else
                material.DisableKeyword(name);
        }
    }

    bool EnsureRenderTarget(ref RenderTexture rt, int width, int height, RenderTextureFormat format, FilterMode filterMode, int depthBits = 0, int antiAliasing = 1)
    {
        if (rt != null && (rt.width != width || rt.height != height || rt.format != format || rt.filterMode != filterMode || rt.antiAliasing != antiAliasing))
        {
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }
        if (rt == null)
        {
            rt = RenderTexture.GetTemporary(width, height, depthBits, format, RenderTextureReadWrite.Default, antiAliasing);
            rt.filterMode = filterMode;
            rt.wrapMode = TextureWrapMode.Clamp;
            return true;
        }
        return false;
    }

    void ReleaseRenderTarget(ref RenderTexture rt)
    {
        if (rt != null)
        {
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }
    }

    void DrawFullscreenQuad()
    {
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        {
            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f); // BL

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 0.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 0.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL
        }
        GL.End();
        GL.PopMatrix();
    }

}
