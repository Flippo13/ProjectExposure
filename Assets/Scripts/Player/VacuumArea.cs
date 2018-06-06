using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumArea : MonoBehaviour {

    [HideInInspector]
    public List<Transform> suckableObjectsList = new List<Transform>();

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Suckable")) {
            suckableObjectsList.Add(other.transform);
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Suckable")) {
            suckableObjectsList.Remove(other.transform);
        }
    }

    public void RemoveTransfromFromList(Transform removedTransform) {
        suckableObjectsList.Remove(removedTransform);
    }

}
