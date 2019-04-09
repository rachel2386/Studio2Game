using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public GameObject panCenter;
    public GameObject panEdge;
    public GameObject farthest;
    public GameObject jumpAnchorY;
    public GameObject foodRoot;

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

    // Start is called before the first frame update
    protected void Start()
    {
        var foodCompenents = foodRoot.GetComponentsInChildren<Food>();
        foreach (var com in foodCompenents)
        {
            foodList.Add(com.gameObject);
            allFoodList.Add(com.gameObject);
        }
           

        oriFoodCount = foodList.Count;
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

    public void FoodListEmpty()
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
