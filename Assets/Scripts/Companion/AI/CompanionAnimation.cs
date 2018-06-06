using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimation : MonoBehaviour {

    public Animator rigAnimator;

    private CompanionNavigator _navigator;

    public void Awake() {
        _navigator = GetComponent<CompanionNavigator>();
    }

    public void Update() {
        AnimateMovement();
    }

    private void AnimateMovement() {
        //set animation according to the companions movement
        if (_navigator.GetAgentVelocity().magnitude <= 0 && GetMovingBool()) {
            SetMovingBool(false);
        } else if (_navigator.GetAgentVelocity().magnitude > 0 && !GetMovingBool()) {
            SetMovingBool(true);
        }
    }

    public void SetMovingBool(bool status) {
        rigAnimator.SetBool("moving", status);
    }

    public bool GetMovingBool() {
        return rigAnimator.GetBool("moving");
    }
}
