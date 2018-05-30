using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//helper class to check for certain controlls
public class CompanionControls : MonoBehaviour {

    public GrabColliderScript rightGrabCollider;
    public InteractScript interact;

    private OVRGrabbable _grabbable;
    private bool _triggerPressed;

    public void Awake() {
        _grabbable = GetComponent<OVRGrabbable>();
        _triggerPressed = false;
    }

    public void Update() {
        UpdateTriggerState();
    }

    //returns true, when the button is pressed down
    public bool CallButtonDown() {
        return OVRInput.GetDown(OVRInput.Button.One);
    }

    //returns true as long as the trigger is held down
    public bool GrabButtonPressed() {
        return OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) != 0;
    }

    //returns true, as long as the trigger is held down
    public bool UseButtonPressed() {
        bool state = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 0;

        if (state && !_triggerPressed) {
            //first frame of pressing
            _triggerPressed = true;
            ResetSuckTime();
        }
        
        return state;
    }

    //returns true, as long as the grab collider collides with the companion collider
    public bool InCollider() {
        return rightGrabCollider.InCollider();
    }

    //use the vacuum mechanic of the companion
    public void UseVacuum() {
        interact.Suck();
    }

    //reset sucking time
    public void ResetSuckTime() {
        interact.SetSuckTime(0);
    }


    //return amount of sucked trash
    public int GetTrashCount() {
        return interact.GetTrashCount();
    }

    //check if the trigger state has to be updated
    private void UpdateTriggerState() {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0 && _triggerPressed) {
            _triggerPressed = false;
        }
    }

}
