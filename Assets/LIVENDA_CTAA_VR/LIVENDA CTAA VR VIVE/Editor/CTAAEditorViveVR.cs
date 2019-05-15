using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CTAAVR_VIVE))]
public class CTAAEditorViveVR : Editor
{
	public Texture2D banner;

	private GUIStyle back1;
	private GUIStyle back2;
	private GUIStyle back3;
	private GUIStyle back4;

	SerializedObject serObj;

	int bannerHeight = 150;

	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width*height];

		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix); 
		result.Apply();
		result.hideFlags = HideFlags.HideAndDontSave;
		return result;
	}



	void OnEnable()
	{
		back1 = new GUIStyle();
		back1.normal.background = MakeTex(600, 1, new Color(0.3f, 0.0f, 0.3f, 0.05f));
		back2 = new GUIStyle();
		back2.normal.background = MakeTex(600, 1, new Color(0.1f, 0.1f, 0.1f, 0.8f));
		back3 = new GUIStyle();
		back3.normal.background = MakeTex(600, 1, new Color(0.0f, 0.0f, 0.0f, 0.8f));
		back4 = new GUIStyle();
		back4.normal.background = MakeTex(600, 1, new Color(0.1f, 0.0f, 0.5f, 0.3f));

		serObj = new SerializedObject(target);

		banner = Resources.Load("CTAA_LOGO", typeof(Texture2D)) as Texture2D;

	}

	public override void OnInspectorGUI()
	{
		serObj.Update();


		var rect = GUILayoutUtility.GetRect(Screen.width - 38, bannerHeight, GUI.skin.box);
		if (banner)
			GUI.DrawTexture(rect, banner, ScaleMode.ScaleToFit);
		
		GUILayout.Space(5);
		GUILayout.BeginVertical(back1);

		this.DrawDefaultInspector ();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		GUILayout.Space(10);

		serObj.ApplyModifiedProperties();

	}


}


