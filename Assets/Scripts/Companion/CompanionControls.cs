using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//helper class to check for certain controlls
public class CompanionControls : MonoBehaviour {

    private InteractScript _interact;
    private bool _triggerPressed;

    public void Awake() {
        _interact = GetComponent<InteractScript>();
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

    //use the vacuum mechanic of the companion
    public void UseVacuum() {
        _interact.Suck();
    }

    //reset sucking time
    public void ResetSuckTime() {
        _interact.SetSuckTime(0);
    }

    //check if the trigger state has to be updated
    private void UpdateTriggerState() {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0 && _triggerPressed) {
            _triggerPressed = false;
        }
    }

}
