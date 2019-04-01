using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace BeatModel {
   // [ExecuteInEditMode]
    public class BeatStructure : MonoBehaviour
    {
        public float musicBPM;

        public GameObject beatButton;
        public GameObject beatButtonClone;
        public List<GameObject> beatlist;
        public int currentBeat;

        public GameObject canvas;
        public GameObject scrollView;
        public GameObject scrollviewContent;
        public List<GameObject> layer;
        public int currentLayer;


        
        public GameObject LayerObj;
        public GameObject cloneLayerObj;
        

        public GameObject musicPlayer;
        public MusicPlayerController musicPlayerController;


        [Range(1, 4)] public int beatInterTimes;

        // beat sprites
        public Sprite grayNormal;
        public Sprite goldNormal;
        public Sprite grayHighlighted;
        public Sprite goldHighlighted;
        public Sprite grayActive;
        public Sprite grayActiveHighlighted;
        public Sprite goldActive;
        public Sprite goldActiveHighlighted;

        public int countSprite = 0;
        public bool countSpriteFlag;

        public bool beatNumberFlag;
        public GameObject beatNumber;
        public GameObject cloneBeatNumber;
        public List<GameObject> beatNumbers;

        //Create Beat buttons based on Size of music in minutes * MusicBPM 
        public void SetBeatGroupSize (){
            for (int i = 0; i < ((musicPlayerController.clip.length / 60) * musicBPM) * beatInterTimes; i++) {

                if (beatNumberFlag)
                {
                    cloneBeatNumber = Instantiate(beatNumber, beatNumber.transform.position, beatNumber.transform.rotation);
                    cloneBeatNumber.transform.SetParent(scrollviewContent.transform);
                    cloneBeatNumber.GetComponent<Text>().text = i.ToString();
                    beatNumbers.Add(cloneBeatNumber);
                }
                else {
                    beatButtonClone = Instantiate(beatButton, beatButton.transform.position, beatButton.transform.rotation);
                    beatButtonClone.transform.SetParent(scrollviewContent.transform);
                    beatButtonClone.GetComponent<BeatUnit>().layer = currentLayer;


                    beatButtonClone.GetComponent<BeatUnitController>().SetBeatId(i);
                    beatlist.Add(beatButtonClone);
                    if (!countSpriteFlag)
                    {

                        if (countSprite < 3)
                        {
                            countSprite++;
                            beatButtonClone.GetComponent<Image>().sprite = grayNormal;
                            SpriteState ss = new SpriteState();
                            ss.pressedSprite = grayHighlighted;
                            beatButtonClone.GetComponent<Button>().spriteState = ss;
                        }
                        else
                        {
                            countSpriteFlag = true;
                            beatButtonClone.GetComponent<Image>().sprite = grayNormal;
                            SpriteState ss = new SpriteState();
                            ss.pressedSprite = grayHighlighted;
                            beatButtonClone.GetComponent<Button>().spriteState = ss;
                        }
                    }
                    else
                    {
                        if (countSprite > 0)
                        {
                            countSprite--;
                            beatButtonClone.GetComponent<Image>().sprite = goldNormal;
                            SpriteState ss = new SpriteState();
                            ss.pressedSprite = goldHighlighted;
                            beatButtonClone.GetComponent<Button>().spriteState = ss;
                        }
                        else
                        {
                            countSpriteFlag = false;
                            beatButtonClone.GetComponent<Image>().sprite = goldNormal;
                            SpriteState ss = new SpriteState();
                            ss.pressedSprite = goldHighlighted;
                            beatButtonClone.GetComponent<Button>().spriteState = ss;
                        }
                    }
                    
                }
                
            }

            if (!beatNumberFlag) {
                currentLayer++;
            }
            
            beatNumberFlag = false;

            // set size of scroll

            scrollviewContent.GetComponent<GridLayoutGroup>().constraintCount = Mathf.RoundToInt(((musicPlayerController.clip.length / 60) * musicBPM) * beatInterTimes);   
        }

        public void SetBeatGroupSizeNumbers()
        {
            for (int i = 0; i < ((musicPlayerController.clip.length / 60) * musicBPM) * beatInterTimes; i++)
            {

                if (beatNumberFlag)
                {
                    cloneBeatNumber = Instantiate(beatNumber, beatNumber.transform.position, beatNumber.transform.rotation);
                    cloneBeatNumber.transform.SetParent(scrollviewContent.transform);
                    cloneBeatNumber.GetComponent<Text>().text = i.ToString();
                    beatNumbers.Add(cloneBeatNumber);
                }
            }

            beatNumberFlag = false;

            // set size of scroll

            scrollviewContent.GetComponent<GridLayoutGroup>().constraintCount = Mathf.RoundToInt(((musicPlayerController.clip.length / 60) * musicBPM) * beatInterTimes);
        }

        public void AddLayer() {
            countSprite = 0;
            cloneLayerObj = Instantiate(LayerObj, LayerObj.transform.position, LayerObj.transform.rotation);
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, scrollView.GetComponent<RectTransform>().sizeDelta.y + 45);
            scrollView.GetComponent<RectTransform>().anchoredPosition = new Vector2(scrollView.GetComponent<RectTransform>().anchoredPosition.x, scrollView.GetComponent<RectTransform>().anchoredPosition.y - 22.5f);
            layer.Add(cloneLayerObj);
            cloneLayerObj.transform.SetParent(canvas.transform);
            if (currentLayer == 0)
            {
                layer[layer.IndexOf(cloneLayerObj)].transform.position = new Vector3(cloneLayerObj.transform.position.x, cloneLayerObj.transform.position.y, cloneLayerObj.transform.position.z);
                cloneLayerObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-245.82f, 21.3f);
            }
            else {
                layer[layer.IndexOf(cloneLayerObj)].transform.position = new Vector3(layer[layer.IndexOf(cloneLayerObj) - 1].transform.position.x, layer[layer.IndexOf(cloneLayerObj) - 1].transform.position.y - 40, layer[layer.IndexOf(cloneLayerObj) - 1].transform.position.z);
            }
            
            SetBeatGroupSize();
            transform.Translate (Vector3.down * 4);
            
        }

        //Change beat color, if is current beat turn blue, else turn white
        public void SetBeatColor() {
            foreach (GameObject text in beatNumbers) {

                text.GetComponent<Text>().color = Color.gray;

                
                
            }

            if (currentBeat > 0) {
                
                beatNumbers[currentBeat].GetComponent<Text>().color = Color.white;
            
                
            }
            
        }

    }

    //Beat button properties
    public class BeatUnit : MonoBehaviour {

        public int id;
        public bool selected;
        public int layer;

        //SETS ID OF BEAT
        public void SetBeatId(int beatId)
        {
            id = beatId;
            SetBeatUi(id);
        }
        //SET ID IN UI
        public void SetBeatUi(int beatId) {
            //GetComponent<Button>().GetComponentInChildren<Text>().text = beatId.ToString();
        }
        //SET OR UNSET BEAT SELECTION STATUS
        public void SetBeatStatus() {
            if (!selected)
            {
                selected = true;

                if (GetComponent<Image>().sprite == GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayNormal) {
                    GetComponent<Image>().sprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayActive;
                    SpriteState ss = new SpriteState();
                    ss.pressedSprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayActiveHighlighted;
                    GetComponent<Button>().spriteState = ss;

                } else if (GetComponent<Image>().sprite == GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldNormal) {
                    GetComponent<Image>().sprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldActive;
                    SpriteState ss = new SpriteState();
                    ss.pressedSprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldActiveHighlighted;
                    GetComponent<Button>().spriteState = ss;

                }
                

                
                
            }
            else {
                selected = false ;
            }
        }
        // SET OR UNSET COLOR IF BEAT IS SELECT OR NOT
        public void SetBeatColor() {
            if (selected)
            {
                if (GetComponent<Image>().sprite == GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayNormal)
                {
                    GetComponent<Image>().sprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayActive;
                    SpriteState ss = new SpriteState();
                    ss.pressedSprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayActiveHighlighted;
                    GetComponent<Button>().spriteState = ss;

                }
                else if (GetComponent<Image>().sprite == GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldNormal)
                {
                    GetComponent<Image>().sprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldActive;
                    SpriteState ss = new SpriteState();
                    ss.pressedSprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldActiveHighlighted;
                    GetComponent<Button>().spriteState = ss;

                }

            }
            else {
                if (GetComponent<Image>().sprite == GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayActive)
                {
                    GetComponent<Image>().sprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayNormal;
                    SpriteState ss = new SpriteState();
                    ss.pressedSprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().grayHighlighted;
                    GetComponent<Button>().spriteState = ss;

                }
                else if (GetComponent<Image>().sprite == GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldActive)
                {
                    GetComponent<Image>().sprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldNormal;
                    SpriteState ss = new SpriteState();
                    ss.pressedSprite = GameObject.Find("BeatController").GetComponent<BeatStructureController>().goldHighlighted;
                    GetComponent<Button>().spriteState = ss;

                }
            }
        }
        //ADD BEAT IN LAYER ARRAY AND ORDER IT. IT'S NECESSARY TO CALL UpdateBeatLayer TWICE
        public void AddBeatToLayer()
        {
            if (GetComponent<BeatUnitController>().selected)
            {
                GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().beatLayer[GetComponent<BeatUnitController>().layer].beatNumber.Add(GetComponent<BeatUnitController>().id);
                GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().UpdateBeatLayer();
                GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().UpdateBeatLayer();
            }
            else {
                GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().beatLayer[GetComponent<BeatUnitController>().layer].beatNumber.Remove(GetComponent<BeatUnitController>().id);
                GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().UpdateBeatLayer();
                GameObject.Find("BeatController").GetComponent<BeatMusicMapController>().UpdateBeatLayer();
            }
            

        }

    }
    //STRUCTURE OF A LAYER
    public class BeatLayerMusic : MonoBehaviour {
        [System.Serializable]
        public struct BeatLayer
        {
            public List<float> beatNumber;
        }
    }

    public class BeatMusicMap: BeatLayerMusic
    {
        

        public List<BeatLayer> beatLayer; // Layer number, representing Instrument
        public List<BeatLayer> beatLayerOrdered;

        public List<float> currentBeatValue;
        public List<float> swapBeatValue;
        public List<float> previousBeatValue;
        public string outputString;
        public string musicName;

        

        public void UpdateBeatLayer()
        {
            foreach (BeatLayer layer in beatLayer)
            {
                foreach (float beat in layer.beatNumber.ToArray())
                {
                    ///print();
                    if (beat != -1)
                    {
                        currentBeatValue[beatLayer.IndexOf(layer)] = beat;
                        if (beat < previousBeatValue[beatLayer.IndexOf(layer)])
                        {
                            layer.beatNumber[layer.beatNumber.IndexOf(previousBeatValue[beatLayer.IndexOf(layer)])] = beat;
                            layer.beatNumber[layer.beatNumber.IndexOf(currentBeatValue[beatLayer.IndexOf(layer)])] = previousBeatValue[beatLayer.IndexOf(layer)];
                        }
                        else if (beat > previousBeatValue[beatLayer.IndexOf(layer)])
                        {
                            layer.beatNumber[layer.beatNumber.IndexOf(currentBeatValue[beatLayer.IndexOf(layer)])] = previousBeatValue[beatLayer.IndexOf(layer)];
                            layer.beatNumber[layer.beatNumber.IndexOf(previousBeatValue[beatLayer.IndexOf(layer)])] = beat;
                        }
                        previousBeatValue[beatLayer.IndexOf(layer)] = beat;
                    }
                }
            }
            SetOrderedBeatList();
        }

        public void SetOrderedBeatList() {
            
            foreach (BeatLayer layer in beatLayer)
            {
                beatLayerOrdered[beatLayer.IndexOf(layer)].beatNumber.Clear();
            }

            foreach (BeatLayer layer in beatLayer)
            {
                foreach (float beat in layer.beatNumber.ToArray())
                {
                    beatLayerOrdered[beatLayer.IndexOf(layer)].beatNumber.Insert(0, beat);
                }
            }
            BeatOrderedClass beatClass = new BeatOrderedClass();
            beatClass.beatOrdered = beatLayerOrdered;
            outputString = JsonUtility.ToJson(beatClass);
            File.WriteAllText("Assets/test.txt", outputString);

        }

        public void LoadArrayFromJson() {
           
            foreach (BeatLayerMusicJson.BeatLayer layerJson in GetComponent<BeatPlayerController>().beatOrdered) {
                foreach (float beatJson in layerJson.beatNumber) {
                    foreach (BeatLayer layerResult in beatLayer)
                    {
                        if (beatLayer.IndexOf(layerResult) == GetComponent<BeatPlayerController>().beatOrdered.IndexOf(layerJson)) {
                            layerResult.beatNumber.Add(beatJson);
                            foreach (GameObject btn in GameObject.FindGameObjectsWithTag("beatBtn")) {
                                if (btn.GetComponent<BeatUnitController>().layer == beatLayer.IndexOf(layerResult) && btn.GetComponent<BeatUnitController>().id == beatJson) {
                                    btn.GetComponent<BeatUnitController>().selected = true;
                                    btn.GetComponent<BeatUnitController>().SetBeatColor();
                                }
                            }
                            UpdateBeatLayer();
                        }
                        
                    }
                }
                
            }
            
            //
            //UpdateBeatLayer();
        }


        public void SaveJsonMusic() {
            File.WriteAllText("Assets/"+musicName+".txt", outputString);
        }

    }
    [System.Serializable]
    public class BeatOrderedClass : BeatLayerMusic
    {
        public List<BeatLayer> beatOrdered;
    }


    public class BeatLayerMusicJson
    {
        [System.Serializable]
        public struct BeatLayer
        {
            public List<float> beatNumber;
        }
        public List<BeatLayer> beatOrdered;
        public string inputString;

        public List<BeatLayer> GetJsonFile(string musicName)
        {
            string loadPath = ("Assets/"+musicName+".txt");
            inputString = File.ReadAllText(loadPath);
            //Debug.Log(inputString);
            beatOrdered = JsonUtility.FromJson<BeatLayerMusicJson>(inputString).beatOrdered;
            
            return beatOrdered;
        }
    }


    [System.Serializable]
    public class BeatOrderedClassJson : BeatLayerMusicJson
    {
        public List<BeatLayer> beatOrderedJson;

    }



    public class BeatPlayer : MonoBehaviour
    {

        BeatLayerMusicJson beatPlayerJson = new BeatLayerMusicJson();
        public List<BeatLayerMusicJson.BeatLayer> beatOrdered;

        public void GetBeatArray(string musicName) {
            beatOrdered = beatPlayerJson.GetJsonFile(musicName);
            try
            {
                GetComponent<BeatMusicMapController>().LoadArrayFromJson();
            }
            catch {
                print("Gameplay");
            }
            
        }

    }

    //USED IN GAMEPLAY
    public class BeatAction : MonoBehaviour
    {


        public float musicBPM;
        public int currentBeat;
        public int beatInterTimes;

        //public GameObject[] layer;

        public GameObject musicPlayer;
        public MusicPlayerController musicPlayerController;
        public BeatPlayerController beatController;
        public List<BeatLayerMusicJson.BeatLayer> beatOrdered;

        public List<EventListenerController> eventListeners;

        public int currentLayer;

        //CALLS BEAT ACTION IF CURRENT BEAT WAS PREVIOUSLY SELECTED
        public void CallBeatAction()
        {
            foreach (BeatLayerMusicJson.BeatLayer layer in beatOrdered ) {
                foreach (float beat in layer.beatNumber)
                {
                    if (currentBeat == beat)
                    {
                        foreach (EventListenerController listener in eventListeners)
                        {
                            try
                            {
                                listener.EventAction(beatOrdered.IndexOf(layer), currentBeat);
                            }
                            catch {
                                print("No Event Listeners Actives");
                            }
                            
                        } 
                    }
                }
            }   
        }
    }

}

