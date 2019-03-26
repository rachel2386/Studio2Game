using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachPlayer : MonoBehaviour
{
    public GameObject ThePlayer;
    public float FollowSpeed;

    void LateUpdate()
    {
        var target = Camera.main.transform.position;
        if (Vector3.Distance(transform.position, target) < 5)
        {
            FollowSpeed = 0.1f;
            transform.position = Vector3.MoveTowards(transform.position, ThePlayer.transform.position, FollowSpeed);
            // ThePlayer.velocity = Vector3.zero
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
