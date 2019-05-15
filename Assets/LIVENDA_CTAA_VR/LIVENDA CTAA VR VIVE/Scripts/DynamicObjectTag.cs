using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Livenda Effects/CTAA_DYNAMIC_OBJECT")]
public class DynamicObjectTag : MonoBehaviour
{
	public static List<DynamicObjectTag> activeObjects = new List<DynamicObjectTag>(128);

	[NonSerialized, HideInInspector] public Mesh mesh;
	[NonSerialized, HideInInspector] public Matrix4x4 localToWorldPrev;
	[NonSerialized, HideInInspector] public Matrix4x4 localToWorldCurr;

	private SkinnedMeshRenderer skinnedMesh = null;
	public bool useSkinnedMesh = false;

	public const int framesNotRenderedThreshold = 60;
	private int framesNotRendered = framesNotRenderedThreshold;

	[NonSerialized] public bool sleeping = false;

	void Start()
	{
		if (useSkinnedMesh)
		{
			var smr = this.GetComponent<SkinnedMeshRenderer>();
			if (smr != null)
			{
				mesh = new Mesh();
				skinnedMesh = smr;
				skinnedMesh.BakeMesh(mesh);
			}
		}
		else
		{
			var mf = this.GetComponent<MeshFilter>();
			if (mf != null)
			{
				mesh = mf.sharedMesh;
			}
		}

		localToWorldCurr = transform.localToWorldMatrix;
		localToWorldPrev = localToWorldCurr;
	}

	void VelocityUpdate()
	{
		if (useSkinnedMesh)
		{
			if (skinnedMesh == null)
			{
				Debug.LogWarning("vbuf skinnedMesh not set", this);
				return;
			}

			if (sleeping)
			{
				skinnedMesh.BakeMesh(mesh);
				mesh.normals = mesh.vertices;
			}
			else
			{
				Vector3[] vs = mesh.vertices;
				skinnedMesh.BakeMesh(mesh);
				mesh.normals = vs;
			}
		}

		if (sleeping)
		{
			localToWorldCurr = transform.localToWorldMatrix;
			localToWorldPrev = localToWorldCurr;
		}
		else
		{
			localToWorldPrev = localToWorldCurr;
			localToWorldCurr = transform.localToWorldMatrix;
		}

		sleeping = false;
	}

	void LateUpdate()
	{
		if (framesNotRendered < framesNotRenderedThreshold)
		{
			framesNotRendered++;
		}
		else
		{
			sleeping = true;
			return;
		}

		VelocityUpdate();
	}

	void OnWillRenderObject()
	{
		if (Camera.current != Camera.main)
			return;

		if (sleeping)
		{
			VelocityUpdate();
		}

		framesNotRendered = 0;
	}

	void OnEnable()
	{
		activeObjects.Add(this);
	}

	void OnDisable()
	{
		activeObjects.Remove(this);
	}
}