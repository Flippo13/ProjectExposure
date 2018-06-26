using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimation : MonoBehaviour {

    public Animator rigAnimator;

    private CompanionNavigator _navigator;
    private bool _playingIdle;
    private bool _playingGrab;

    public void Awake() {
        _navigator = GetComponent<CompanionNavigator>();

        _playingIdle = false;
    }

    public void Update() {
        AnimateMovement();
        CheckForIdle();
    }

    private void AnimateMovement() {
        //set animation according to the companions movement
        if (_navigator.GetAgentVelocity().magnitude <= 0 && GetMovingBool()) {
            SetMovingBool(false);
        } else if (_navigator.GetAgentVelocity().magnitude > 0 && !GetMovingBool()) {
            SetMovingBool(true);

            //rotation
            float rotationAngle = _navigator.GetRotationAngle(1f);

            if(rotationAngle > 0f) {
                //rotating right
                rigAnimator.SetBool("turning_right", false);
                rigAnimator.SetBool("turning_left", true);
            } else if(rotationAngle < 0f) {
                //rotating left
                rigAnimator.SetBool("turning_left", false);
                rigAnimator.SetBool("turning_right", true);
            } else {
                //not rotating
                rigAnimator.SetBool("turning_left", false);
                rigAnimator.SetBool("turning_right", false);
            }
        }
    }

    private void CheckForIdle() {
        if (!_playingIdle) return;

        if (rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("LookAround") || rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("Farting") || rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("Burp") || rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("Done")) {
            rigAnimator.SetInteger("idle", 0);
            _playingIdle = false;
        }
    }

    public void SetMovingBool(bool status) {
        rigAnimator.SetBool("moving", status);
    }

    public bool GetMovingBool() {
        return rigAnimator.GetBool("moving");
    }

    public void SetRandomIdle() {
        float rnd = Random.Range(1, 3);
        rigAnimator.SetInteger("idle", (int)Mathf.Round(rnd));
        _playingIdle = true;
    }

    public void SetGrabbingVaccumTrigger() {
        if (rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScoopVacuum") || _playingGrab) return; //dont play it if it is already playing

        rigAnimator.SetTrigger("grab_vacuum");
        _playingGrab = true;
    }

    public void SetAnimationTrigger(string triggerName) {
        if (triggerName == "") return;

        rigAnimator.SetTrigger(triggerName);
    }

    public void SetPlayingGrab(bool status) {
        _playingGrab = status;
    }

    public bool IsPlayingIdle() {
        return _playingIdle;
    }

    public bool VacuumHandDone() {
        return rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("VacuumHandDone");
    }
}
