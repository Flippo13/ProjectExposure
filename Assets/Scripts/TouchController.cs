using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

    public OVRInput.Controller controller;

    private Rigidbody _rigidbody;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Update() {
        transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);

        if(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0 || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) != 0) {
            _rigidbody.AddForce(Camera.main.transform.forward);
        }
    }
}
