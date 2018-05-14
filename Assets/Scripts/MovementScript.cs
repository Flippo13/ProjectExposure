using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour {

    public float speed;
    public Transform leftHandAnchor;

    public bool underwaterMovement;

    private Rigidbody _rigidbody;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Update() {
        //if either of the index buttons is pressed, move into the direction the controllers are pointing to
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0 || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) != 0) {
            Vector3 deltaVec = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) - OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch); //vector pointing from the left to the right hand
            Vector3 movementDir;

            //might not quite work, since it probably uses wrong 
            if(underwaterMovement) {
                movementDir = Vector3.Cross(leftHandAnchor.up, deltaVec); //getting the movement direction in all directions taking the controller rotation into account
            } else {
                movementDir = Vector3.Cross(Vector3.up, deltaVec); //getting the movement direction on the ground
            }

            _rigidbody.AddForce(movementDir.normalized * speed); //move into the controller direction

            Debug.Log("Moving towards direction: " + movementDir);

            //TODO: slowly rotate the player around the Y axis towards the movement direction
        }
    }
}
