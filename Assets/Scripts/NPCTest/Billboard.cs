using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
         {
             var target = Camera.main.transform.position;
             if (Vector3.Distance(transform.position, target) < 3){
                 target.y = transform.position.y;
                 transform.LookAt(target);
             }
            
         }
}
