using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour {

    public Transform companionTransform;
    public bool useQuaternions;

    public void Update() {
        if(useQuaternions) {
            //try 1: Quaternions
            Vector3 deltaVec = companionTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(deltaVec);
        } else {
            //try 2: Look At
            transform.LookAt(companionTransform.position);
        }

    }
}
