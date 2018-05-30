using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumArea : MonoBehaviour {

    private CapsuleCollider col;
    public LayerMask suckableLayer;

    [HideInInspector]
    public List<Transform> suckableObjectsList = new List<Transform>();

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == suckableLayer) {
            suckableObjectsList.Add(other.transform);
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == suckableLayer) {
            suckableObjectsList.Remove(other.transform);
        }
    }

    public void RemoveTransfromFromList(Transform removedTransform) {
        suckableObjectsList.Remove(removedTransform);
    }

}
