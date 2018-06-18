using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//helper class to check for certain controlls
public class CompanionControls : MonoBehaviour {

    public VacuumScript interact;

    //returns true, when the button is pressed down
    public bool CallButtonDown() {
        return OVRInput.GetDown(OVRInput.Button.One);
    }

    //return amount of sucked trash
    public int GetTrashCount() {
        return interact.GetTrashCount();
    }

}
