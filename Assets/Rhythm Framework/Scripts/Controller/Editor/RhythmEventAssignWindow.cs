using UnityEngine;
using UnityEditor;

public class RhythmEventAssignWindow : EditorWindow
{

    public GameObject[] eventObject;
    public AudioClip clip;
    public MusicPlayerController musicPlayerController;
    public BeatStructureController beatStructureController;
    public BeatPlayerController beatPlayerController;
    public BeatMusicMapController beatMusicMapController;
    public BeatActionController beatActionController;
    public int musicBPM;

    public string JSONNameToLoad;

    public string JSONNameToSave;

    [Range(1, 4)] public int beatInterTimes = 1;

    [Range(0, 20)] public float offset = 0;
    Vector2 scrollPos;

    [MenuItem("Rhythm/Project Settings")]
    public static void ShowWindow() {
        //RhythmEventAssignWindow window =  GetWindow<RhythmEventAssignWindow>(typeof(RhythmEventAssignWindow), true, "Rhythm Project Settings");

        RhythmEventAssignWindow window = (RhythmEventAssignWindow)GetWindow(typeof(RhythmEventAssignWindow), false, "Rhythm Project Settings");


    }

    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(350));

        
        

        // ShowWindow();
        GUILayout.Label("Properties of Music",EditorStyles.boldLabel);
        GUILayout.Label("Set your Audio Clip");
        clip = (AudioClip) EditorGUILayout.ObjectField(clip, typeof(AudioClip), true);
        GUILayout.Label("Set Music BPM");
        musicBPM = EditorGUILayout.IntField(musicBPM);
        GUILayout.Label("\nSet Music BPM INTERTIMES");
        GUILayout.Label("Multiply beats to achieve more moments between beats.\n Default is 1", EditorStyles.miniBoldLabel);
        beatInterTimes = Mathf.RoundToInt( EditorGUILayout.Slider(beatInterTimes, 1, 4));

        GUILayout.Label("\nSet Music offset");
        GUILayout.Label("Delay between music and beat generation. DEFAULT IS: 0", EditorStyles.miniBoldLabel);
        offset = EditorGUILayout.Slider(offset, 0, 20);
        GameObject.Find("AudioPlayer").GetComponent<MusicPlayerController>().offset = offset;
        GUILayout.Label("Save/Load JSON File", EditorStyles.boldLabel);
        GUILayout.Label("Insert JSON Name to load");
        JSONNameToLoad = EditorGUILayout.TextField(JSONNameToLoad);

        if (GUILayout.Button("LOAD"))
        {
            
            foreach (BeatMusicMapController.BeatLayer layer in GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().beatLayerOrdered) {
                layer.beatNumber.Clear();
            }
            foreach (BeatMusicMapController.BeatLayer layer in GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().beatLayer)
            {
               layer.beatNumber.Clear();
            }
            beatPlayerController.GetBeatArray(JSONNameToLoad);
        }

        GUILayout.Label("Insert JSON Name to save");
        JSONNameToSave = EditorGUILayout.TextField(JSONNameToSave);
        if (GUILayout.Button("SAVE")) {
            beatMusicMapController.SaveJsonMusic();
        }

        

        EditorGUILayout.EndScrollView();
    }

    public void Update()
    {
        musicPlayerController = GameObject.Find("AudioPlayer").GetComponent<MusicPlayerController>();
        beatStructureController = GameObject.Find("BeatController").GetComponent<BeatStructureController>();
        beatPlayerController = GameObject.Find("BeatController").GetComponent<BeatPlayerController>();
        beatMusicMapController = GameObject.Find("BeatController").GetComponent<BeatMusicMapController>();
        beatActionController = GameObject.Find("BeatController").GetComponent<BeatActionController>();
        musicPlayerController.clip = clip;
        if (beatStructureController != null) {
            beatStructureController.musicBPM = musicBPM;
            beatStructureController.beatInterTimes = beatInterTimes;
        } else if (beatActionController != null) {
            beatActionController.musicBPM = musicBPM;
            beatActionController.beatInterTimes = beatInterTimes;
        }
        if (beatMusicMapController != null)
        {
            beatMusicMapController.musicName = JSONNameToSave;
        }
        
    }
}
