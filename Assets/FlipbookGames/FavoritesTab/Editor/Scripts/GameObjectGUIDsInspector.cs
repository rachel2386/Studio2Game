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

using UnityEditor;

[CustomEditor(typeof(FG_GameObjectGUIDs))]
public class GameObjectGUIDsInspector : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox(@"This is an auto-generated (and auto-destroyed) game object, managed and used by Favorites Tab[s] extension. It has no functionality except that it stores data used by the extension. It stores references to game objects in current scene so that your selection of favorite game objects can be persistent.
			
You can delete this game object if you want to share your project with other users who are not using the Favorites Tab[s], just keep in mind that such action will clear your (and your teammates') list of favorite game objects in this scene.
			
Alternatively, feel free to share the 'FlipbookGames/Common' folder from this Project with others and keep this game object in the scene. That would avoid 'missing behaviour script' error, and you'd save your list of favorite game object.", MessageType.Info, true);
	}
}
