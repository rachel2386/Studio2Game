/* Favorites Tab[s] for Unity
 * version 1.2.19, November 2017
 * Copyright © 2012-2017, by Flipbook Games
 * 
 * Your personalized list of favorite assets and scene objects
 * 
 * Follow @FlipbookGames on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join Unity forum discusion http://forum.unity3d.com/threads/149856
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

using UnityEngine;

public class FG_GameObjectGUIDs : MonoBehaviour
{
	[System.NonSerialized]
	public static bool _dirty = true;

	FG_GameObjectGUIDs() { _dirty = true; }
	void Awake() { _dirty = true; }
	void OnDisable() { _dirty = true; }
	void OnDestroy() { _dirty = true; }
	
	[HideInInspector]
	public string[] guids = new string[0];
	[HideInInspector]
	public UnityEngine.Object[] objects = new UnityEngine.Object[0];
}
