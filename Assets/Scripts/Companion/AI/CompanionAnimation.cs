using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimation : MonoBehaviour {

    public Animator rigAnimator;
    public Animator transformationAnimation;

	public void SetVacuumState(bool state) {
        transformationAnimation.SetBool("Vacuum_state", state);
    }

    public bool TransformedBack() {
        return transformationAnimation.GetCurrentAnimatorStateInfo(0).IsName("Static");
    }

    public void SetMovingBool(bool status) {
        rigAnimator.SetBool("moving", status);
    }


}
