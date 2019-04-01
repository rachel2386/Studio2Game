using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerModel {
    public class PlayerMovement : MonoBehaviour
    {
        public float speed;
        public int lanePosition;

        public bool buttonReleased;

        public void HandleInput() {
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                buttonReleased = true;
            }
            if (lanePosition == 0) {
                
                if (Input.GetAxisRaw("Horizontal") > 0 && buttonReleased) {
                    lanePosition = 1;
                    buttonReleased = false;

                }else if (Input.GetAxisRaw("Horizontal") < 0 && buttonReleased) {
                    lanePosition = -1;
                    Move(lanePosition);
                    buttonReleased = false;
                }
            } else if (lanePosition > 0) {
                if (Input.GetAxisRaw("Horizontal") < 0 && buttonReleased)
                {
                    lanePosition = 0;
                    Move(lanePosition);
                    buttonReleased = false;
                }
            } else if (lanePosition < 0) {
                if (Input.GetAxisRaw("Horizontal") > 0 && buttonReleased)
                {
                    lanePosition = 0;
                    Move(lanePosition);
                    buttonReleased = false;
                }
            }

            

            if (transform.position.x > lanePosition && lanePosition == -1) {
                Move(lanePosition);
            } else if (transform.position.x != 0 && lanePosition == 0) {
                Move(lanePosition);
            }
            else if (transform.position.x < lanePosition && lanePosition == 1)
            {
                Move(lanePosition);
            }
            
        }

        public void Move(int destination){
            if (destination < transform.position.x && lanePosition == -1) {
                transform.Translate(-Vector3.right * speed * Time.deltaTime);
            } else if (destination > transform.position.x && lanePosition == -1) {
                transform.position = new Vector3(-1, 1, -4);
            } else if (destination < transform.position.x && lanePosition == 0) {
                transform.Translate(-Vector3.right * speed * Time.deltaTime);
            } 

            if (destination > transform.position.x && lanePosition == 1)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
            else if (destination < transform.position.x && lanePosition == 1)
            {
                transform.position = new Vector3(1, 1, -4);
            }
            else if (destination > transform.position.x && lanePosition == 0)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
           


        }
    }
}

