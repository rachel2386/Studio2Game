using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StreetModel {
    public class StreetMovement : MonoBehaviour
    {
        public float speed;

        public void MoveStreet() {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
    }
    public class StreetPath:MonoBehaviour{

        public GameObject cloneStreet;
        public GameObject streetPrefab;

        public void PlaceAnother() {
            cloneStreet = Instantiate(streetPrefab, new Vector3(transform.parent.transform.position.x, transform.parent.transform.position.y, transform.parent.transform.position.z + 15), transform.parent.transform.rotation);
            cloneStreet.GetComponentInChildren<StreetPathController>().streetPrefab = streetPrefab;
        }

    }
}

