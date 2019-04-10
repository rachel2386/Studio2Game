using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;
    [SerializeField] private float moveSpeed;

    private CharacterController charController;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName) * moveSpeed;
        float vertInput = Input.GetAxis(verticalInputName) * moveSpeed;
        
        //specify the movement direction & amount
        //W & S
        Vector3 forwardMovement = transform.forward * vertInput;
        //A & D
        Vector3 rightMovement = transform.right * horizInput;

        //SimpleMove applies totalTimeInSeconds.deltatime
        charController.SimpleMove(forwardMovement + rightMovement);
    }
}
