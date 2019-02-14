/* Favorites Tab[s] for Unity
 * version 1.2.19, November 2017
 * Copyright © 2012-2017, by Flipbook Games
 * 
 * Your personalized list of favorite assets and scene objects
 * 
 * Follow @FlipbookGames at http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join Unity forum discusion http://forum.unity3d.com/threads/149856
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

using UnityEditor;
using UnityEngine;

public class AboutFavoritesTab : EditorWindow
{
	private GUIStyle textStyle;
	private GUIStyle bigTextStyle;
	private GUIStyle miniTextStyle;
	private Texture2D flipbookLogo;

	private void OnEnable()
	{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		title = "About";
#else
		titleContent.text = "About";
#endif
		minSize = new Vector2(265f, 155f);
		maxSize = new Vector2(265f, 155.1f);
	}

	void Initialize()
	{
		textStyle = new GUIStyle(EditorStyles.boldLabel);
		textStyle.alignment = TextAnchor.UpperCenter;
		
		bigTextStyle = new GUIStyle(EditorStyles.boldLabel);
		bigTextStyle.fontSize = 24;
		bigTextStyle.alignment = TextAnchor.UpperCenter;
		
		miniTextStyle = new GUIStyle(EditorStyles.miniLabel);
		miniTextStyle.alignment = TextAnchor.UpperCenter;

		flipbookLogo = FavoritesTab.createdByFlipbookGames;
	}

	private void OnGUI()
	{
		if (textStyle == null)
			Initialize();

		EditorGUILayout.BeginVertical();

		GUILayout.Box("Favorites Tab[s]", bigTextStyle);
		GUILayout.Label("Version " + FavoritesTab.GetVersionString(), textStyle);

		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		if (GUILayout.Button(flipbookLogo, GUIStyle.none))
		{
			Application.OpenURL("http://flipbookgames.com/");
		}
		if (Event.current.type == EventType.Repaint)
		{
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close"))
		{
			Close();
		}
		GUILayout.EndVertical();
		GUILayout.Space(16f);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(4f);
		GUILayout.Label("\xa9 Flipbook Games. All Rights Reserved.", miniTextStyle);

		EditorGUILayout.EndVertical();
	}
}
