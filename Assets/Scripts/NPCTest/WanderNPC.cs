using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WanderNPC : MonoBehaviour

{
    public float moveSpeed = 3f;
    public float rotSpeed = 100f;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //use two equations as "is equal"
        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }
        if (isRotatingRight == true)
        {
            //if uses right then it'll do a backflip
            //uses deltatime to normalize (not let the NPC move with the frame)
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }
        if (isRotatingLeft == true)
        {
            //rotate opposite direction
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }
        if (isWalking == true)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator Wander()
    {
        //the amount of rotating time
        int rotTime = Random.Range(1, 3);
        //the time in between each time it rotates
        int rotateWait = Random.Range(1, 4);
        //kind of like a bool
        int rotateLorR = Random.Range(1, 2);
        //the time in between walking and waiting
        int walkWait = Random.Range(1, 4);
        //the amount of walking time
        int walkTime = Random.Range(1, 5);

        isWandering = true;
        
        //stopping it from running every single frame
        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        if (rotateLorR == 1)
        {
            //don't want the NPC to rotate at all when walking forward
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        if (rotateLorR == 2)
        {
            //don't want the NPC to rotate at all when walking forward
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        isWandering = false;
    }
}
