using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineScanner : MonoBehaviour {

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == Layers.Suckable) {
            //trash
            Material material = other.gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_InRange", 1f); //true

        } else if(other.gameObject.GetComponent<OVRGrabbable>()) {
            //interactable
            Material material = other.gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_InRange", 1f); //true

        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == Layers.Suckable) {
            //trash
            Material material = other.gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_InRange", 0f); //false

        } else if (other.gameObject.GetComponent<OVRGrabbable>()) {
            //interactable
            Material material = other.gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_InRange", 0f); //false

        }
    }
}
