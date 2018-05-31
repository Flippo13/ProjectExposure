using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimation : MonoBehaviour {

    public Animator rigAnimator;
    public Animator transformationAnimation;

	public void SetVaccumState(bool state) {
        transformationAnimation.SetBool("Vacuum_state", state);
    }
}
