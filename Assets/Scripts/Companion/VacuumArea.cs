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
            Debug.Log("Adding item");
            suckableObjectsList.Add(other.transform);
        }
    }

    public void OnTriggerStay(Collider other) {
        if(other.gameObject.layer == suckableLayer && !suckableObjectsList.Contains(other.transform)) {
            Debug.Log("Adding item in stay");
            suckableObjectsList.Add(other.transform);
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == suckableLayer) {
            Debug.Log("Removing item");
            suckableObjectsList.Remove(other.transform);
        }
    }

    public void RemoveTransfromFromList(Transform removedTransform) {
        suckableObjectsList.Remove(removedTransform);
    }

}
