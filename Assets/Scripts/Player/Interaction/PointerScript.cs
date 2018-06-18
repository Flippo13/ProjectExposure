using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour {

    public Transform companionTransform;
    public bool useQuaternions;

    //help :(

    public void Update() {
        if(useQuaternions) {
            //try 1: Quaternions
            Vector3 deltaVec = new Vector3(companionTransform.position.x, companionTransform.position.y, transform.position.z) - transform.position;
            transform.rotation = Quaternion.LookRotation(deltaVec);
        } else {
            //try 2: Look At
            Vector3 fakeDestination = new Vector3(companionTransform.position.x, transform.position.y, companionTransform.position.z); //ignore y axis rotation
            transform.LookAt(fakeDestination);
        }

    }
}
