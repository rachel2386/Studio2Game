using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DetectNPC : MonoBehaviour
{
    private NavMeshAgent _agent;
    public GameObject Player;
    public float EnemyDistanceRun = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);
        Debug.Log("Distance: " + distance);
        //run away from player
        if (distance < EnemyDistanceRun)
        {
            //vector player to me
            Vector3 dirtoPlayer = transform.position - Player.transform.position;
            Vector3 newPos = transform.position + dirtoPlayer;
            _agent.SetDestination(newPos);
        }
    }
}
