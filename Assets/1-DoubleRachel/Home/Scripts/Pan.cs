using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public GameObject panCenter;
    public GameObject panEdge;
    public GameObject farthest;
    public GameObject jumpAnchorY;
    public GameObject[] foodRoot;
    public GameObject[] pickRoot;
    

    protected List<GameObject> foodList = new List<GameObject>();
    protected List<GameObject> allFoodList = new List<GameObject>();

    float radius;
    protected int oriFoodCount = 1;
    public float Radius
    {
        get { return radius; }
    }

    float max;
    public float Max
    {
        get { return max; }
    }


    private void Awake()
    {
        radius = ShyMiscTool.GetPlaneDistance(panCenter.transform.position, panEdge.transform.position);
        max = ShyMiscTool.GetPlaneDistance(panCenter.transform.position, farthest.transform.position);
    }

    protected bool useFirstSet = true;

    // Start is called before the first frame update
    protected void Start()
    {
        useFirstSet = HomeSceneManager.IntoIndex <= 0;
 
        // Pick food
        foreach (var go in pickRoot)
            go.SetActive(false);

        int pickRootIndex = useFirstSet ? 0 : 1;
        pickRoot[pickRootIndex].SetActive(true);


        // food in pan
        int foodRootIndex = useFirstSet ? 0 : 1;
        var foodCompenents = foodRoot[foodRootIndex].GetComponentsInChildren<Food>();
        foreach (var com in foodCompenents)
        {
            foodList.Add(com.gameObject);
            allFoodList.Add(com.gameObject);
        }          

        oriFoodCount = foodList.Count;

        foreach (var go in foodRoot)
            go.SetActive(false);
        foodRoot[foodRootIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SendEventGoodMode()
    {
        foreach (var food in foodList)
        {
            food.MySendEventToAll("FOOD_TOSS");
        }
    }


    bool firstFoundFoodListEmpty = true;
    public void SendEventBadMode()
    {
        SpiltFood();

        foreach (var food in foodList)
        {
            food.MySendEventToAll("FOOD_TOSS");
        }

        if(foodList.Count == 0 && firstFoundFoodListEmpty)
        {
            firstFoundFoodListEmpty = false;
            FoodListEmpty();
        }
    }

    // In End
    public virtual void FoodListEmpty()
    {
        
    }

    // Return and remove the food from the list
    public GameObject PopRandomFood()
    {
        if (foodList.Count == 0)
            return null;

        var ranIndex = Random.Range(0, foodList.Count);

        var ret = foodList[ranIndex];
        foodList.RemoveAt(ranIndex);

        return ret;
    }

    public void SpiltFood()
    {
        var spiltFood = PopRandomFood();
        if (spiltFood)
        {
            spiltFood.MySendEventToAll("SPILT");
            spiltFood.transform.SetParent(null);
        }

    }

    
}
