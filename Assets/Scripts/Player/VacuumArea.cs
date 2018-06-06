using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumArea : MonoBehaviour {

    [HideInInspector]
    public List<Transform> suckableObjectsList = new List<Transform>();

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Suckable")) {
            suckableObjectsList.Add(other.transform);

            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Suckable")) {
            suckableObjectsList.Remove(other.transform);

            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
    }

    public void RemoveTransfromFromList(Transform removedTransform) {
        suckableObjectsList.Remove(removedTransform);
    }

}
