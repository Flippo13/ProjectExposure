using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionDestination : MonoBehaviour {

    public Transform centerEyeTransform;

    public void Update() {
        UpdateYRotation();
    }

    private void UpdateYRotation() {
        //update only y rotation
        transform.rotation = centerEyeTransform.rotation;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public Vector3 GetDestinationPosition(float range) {
        //return a position that is range away from the front of the player
        return centerEyeTransform.position + transform.forward.normalized * range;
    }

    public Vector3 GetPosition() {
        return centerEyeTransform.position;
    }
}
