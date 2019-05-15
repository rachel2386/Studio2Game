//===============================================================
//CTAA VR VELOCITY RENDER OCULUSV201A
//Copyright Livenda Labs 2017-2019
//VIRTUAL REALITY VERSION
//===============================================================

using System;
using UnityEngine;


public class CTAAVelocity_Oculus : MonoBehaviour
{
	#if UNITY_PS4
	private const RenderTextureFormat velocityFormat = RenderTextureFormat.RGHalf;
	#else
	private const RenderTextureFormat velocityFormat = RenderTextureFormat.RGFloat;
	#endif


	public Camera.StereoscopicEye VRCameraEYE;
	private Material velocityMaterial;
	private Matrix4x4? velocityViewMatrix;

	[HideInInspector, NonSerialized] public RenderTexture velocityBuffer;

	private float timeScaleNextFrame;
	public float timeScale { get; private set; }

	private Camera _camera;

	void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	void Start()
	{
		timeScaleNextFrame = Time.timeScale;
	}

	private void EnsureMaterial(ref Material material, Shader shader)
	{
		if (shader != null)
		{
			if (material == null || material.shader != shader)
				material = new Material(shader);
			if (material != null)
				material.hideFlags = HideFlags.DontSave;
		}
		else
		{
			Debug.LogWarning("missing shader", this);
		}
	}

	private void EnsureKeyword(Material material, string name, bool enabled)
	{
		if (enabled != material.IsKeywordEnabled(name))
		{
			if (enabled)
				material.EnableKeyword(name);
			else
				material.DisableKeyword(name);
		}
	}

	private void EnsureRenderTarget(ref RenderTexture rt, int width, int height, RenderTextureFormat format, FilterMode filterMode, int depthBits = 0)
	{
		if (rt != null && (rt.width != width || rt.height != height || rt.format != format || rt.filterMode != filterMode))
		{
			RenderTexture.ReleaseTemporary(rt);
			rt = null;
		}
		if (rt == null)
		{
			rt = RenderTexture.GetTemporary(width, height, depthBits, format);
			rt.filterMode = filterMode;
			rt.wrapMode = TextureWrapMode.Clamp;
		}
	}

	private void FullScreenQuad()
	{
		GL.PushMatrix();
		GL.LoadOrtho();

		GL.Begin(GL.QUADS);
		GL.MultiTexCoord2(0, 0.0f, 0.0f);
		GL.Vertex3(0.0f, 0.0f, 0.0f); 

		GL.MultiTexCoord2(0, 1.0f, 0.0f);
		GL.Vertex3(1.0f, 0.0f, 0.0f); 

		GL.MultiTexCoord2(0, 1.0f, 1.0f);
		GL.Vertex3(1.0f, 1.0f, 0.0f); 

		GL.MultiTexCoord2(0, 0.0f, 1.0f);
		GL.Vertex3(0.0f, 1.0f, 0.0f); 

		GL.End();
		GL.PopMatrix();
	}

	private Matrix4x4 GetPerspectiveProjection(float left, float right, float bottom, float top, float near, float far)
	{
		float x = (2.0f * near) / (right - left);
		float y = (2.0f * near) / (top - bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0f * far * near) / (far - near);
		float e = -1.0f;

		Matrix4x4 m = new Matrix4x4();
		m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
		m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
		m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
		m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
		return m;
	}

	private Matrix4x4 GetPerspectiveProjection(Camera camera)
	{
		return GetPerspectiveProjection(camera, 0f, 0f);
	}

	private Matrix4x4 GetPerspectiveProjection(Camera camera, float tsOXp, float tsOYp)
	{
		if (camera == null)
			return Matrix4x4.identity;

		float oneExtentY = Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
		float oneExtentX = oneExtentY * camera.aspect;
		float tsXp = oneExtentX / (0.5f * camera.pixelWidth);
		float tsYp = oneExtentY / (0.5f * camera.pixelHeight);
		float oneJitterX = tsXp * tsOXp;
		float oneJitterY = tsYp * tsOYp;

		float cf = camera.farClipPlane;
		float cn = camera.nearClipPlane;
		float xm = (oneJitterX - oneExtentX) * cn;
		float xp = (oneJitterX + oneExtentX) * cn;
		float ym = (oneJitterY - oneExtentY) * cn;
		float yp = (oneJitterY + oneExtentY) * cn;

		return GetPerspectiveProjection(xm, xp, ym, yp, cn, cf);
	}

	private Vector4 GetPerspectiveProjectionCornerRay(Camera camera)
	{
		return GetPerspectiveProjectionCornerRay(camera, 0f, 0f);
	}

	private Vector4 GetPerspectiveProjectionCornerRay(Camera camera, float tsOXp, float tsOYp)
	{
		if (camera == null)
			return Vector4.zero;

		float oneExtentY = Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
		float oneExtentX = oneExtentY * camera.aspect;
		float tsXp = oneExtentX / (0.5f * camera.pixelWidth);
		float tsYp = oneExtentY / (0.5f * camera.pixelHeight);
		float oneJitterX = tsXp * tsOXp;
		float oneJitterY = tsYp * tsOYp;

		return new Vector4(oneExtentX, oneExtentY, oneJitterX, oneJitterY);
	}

	void OnPostRender()
	{
		Matrix4x4 matrixxTmp = base.GetComponent<Camera>().GetStereoProjectionMatrix (VRCameraEYE);

		base.GetComponent<Camera>().ResetProjectionMatrix();
		base.GetComponent<Camera>().ResetStereoProjectionMatrices();
		
		EnsureMaterial(ref velocityMaterial, Shader.Find("Hidden/CTAA_Velocity_Oculus_VR") );

		if (_camera.orthographic || _camera.depthTextureMode == DepthTextureMode.None || velocityMaterial == null)
		{
			if (_camera.depthTextureMode == DepthTextureMode.None)
				_camera.depthTextureMode = DepthTextureMode.Depth;
			return;
		}

		timeScale = timeScaleNextFrame;
		timeScaleNextFrame = (Time.timeScale == 0f) ? timeScaleNextFrame : Time.timeScale;

		int bufferW = _camera.pixelWidth;
		int bufferH = _camera.pixelHeight;

		EnsureRenderTarget(ref velocityBuffer, bufferW, bufferH, velocityFormat, FilterMode.Point, 16);

		Matrix4x4 cameraP = _camera.projectionMatrix;
		Matrix4x4 cameraV = _camera.worldToCameraMatrix;
		Matrix4x4 cameraVP = cameraP * cameraV;


		if (velocityViewMatrix == null)
			velocityViewMatrix = cameraV;

		RenderTexture activeRT = RenderTexture.active;
		RenderTexture.active = velocityBuffer;
		{
			GL.Clear(true, true, Color.black);

			const int preRenderModePass  = 0;
			const int vertRenderModePass = 1;
			const int skRenderModePass   = 2;
		
			//var jitter = GetComponent<CTAAVRJitter_Oculus>();
			//if (jitter != null)
			//	velocityMaterial.SetVector("_Corner", GetPerspectiveProjectionCornerRay(_camera, jitter.getActiveSampleX(), jitter.getActiveSampleY()));
			//else
			velocityMaterial.SetVector("_Corner", GetPerspectiveProjectionCornerRay(_camera));

			velocityMaterial.SetMatrix("_CurrV", cameraV);
			velocityMaterial.SetMatrix("_CurrVP", cameraVP);
			velocityMaterial.SetMatrix("_PrevVP", cameraP * velocityViewMatrix.Value);
			velocityMaterial.SetPass(preRenderModePass);
			FullScreenQuad();

			var obs = CTAA_Dynamic_Tag.activeObjects;

			for (int i = 0, n = obs.Count; i != n; i++)
			{
				var ob = obs[i];
				if (ob != null && ob.sleeping == false && ob.mesh != null)
				{
					velocityMaterial.SetMatrix("_CurrM", ob.localToWorldCurr);
					velocityMaterial.SetMatrix("_PrevM", ob.localToWorldPrev);
					velocityMaterial.SetPass(ob.useSkinnedMesh ? skRenderModePass : vertRenderModePass);

					for (int j = 0; j != ob.mesh.subMeshCount; j++)
					{
						Graphics.DrawMeshNow(ob.mesh, Matrix4x4.identity, j);

					}

				}
			}

		}

		RenderTexture.active = activeRT;

		velocityViewMatrix = cameraV;

		base.GetComponent<Camera>().SetStereoProjectionMatrix (VRCameraEYE, matrixxTmp);
	}
}