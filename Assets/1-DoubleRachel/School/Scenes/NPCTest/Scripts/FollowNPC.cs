using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowNPC : MonoBehaviour
{
    public GameObject ThePlayer;
    public float TargetDistance;
    public float AllowedDistance = 5;
    public GameObject TheNPC;
    public float FollowSpeed;
    //npc shooting raycast towards player to see how far away they are
    public RaycastHit Shot;

    // Update is called once per frame
    void Update()
    {
        //make sure npc is looking at player
        transform.LookAt(ThePlayer.transform);
        //state raycast shoot out from wherever the script is attached to
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Shot))
        {
            TargetDistance = Shot.distance;
            //define within range or out of range
            if (TargetDistance >= AllowedDistance)
            {
                FollowSpeed = 0.1f;
                //play animation
                //TheNPC.GetComponent<Animator>().Play("run");
                //transform position of the attached object and move it towards where it's looking
                transform.position = Vector3.MoveTowards(transform.position, ThePlayer.transform.position, FollowSpeed);
            }
            else
            {
                FollowSpeed = 0;
                //TheNPC.GetComponent<Animator>().Play("wait");
            }
        }
    }
}
