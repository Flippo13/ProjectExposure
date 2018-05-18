using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionControls : MonoBehaviour {

    private CompanionAI _ai;
    private CompanionDebug _debug;

    public void Awake() {
        _ai = GetComponent<CompanionAI>();
    }

    public void Update() {
        if (OVRInput.GetDown(OVRInput.Button.One)) {
            Call(_ai.GetCurrentState());
        }
    }

    private void Call(CompanionState state) {
        if(state == CompanionState.Inactive) {
            //activate the companion
            _ai.SetState(CompanionState.Instructing);
            _debug.ApplyState(_ai.GetCurrentState());
        } else if(state != CompanionState.Instructing && state != CompanionState.Useable) {
            //call the companion
        }
    }

}
