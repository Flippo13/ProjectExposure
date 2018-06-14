using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimation : MonoBehaviour {

    public Animator rigAnimator;

    private CompanionNavigator _navigator;
    private bool _playingIdle;

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
        }
    }

    private void CheckForIdle() {
        if (!_playingIdle) return;

        if (rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("Done")) {
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
        if (rigAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScoopVacuum")) return; //dont play it if it is already playing

        rigAnimator.SetTrigger("grab_vacuum");
    }

    public void SetAnimationTrigger(string triggerName) {
        if (triggerName == "") return;

        rigAnimator.SetTrigger(triggerName);
    }
}
