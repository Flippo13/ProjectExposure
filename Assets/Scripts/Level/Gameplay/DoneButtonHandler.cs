using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoneButtonHandler : MonoBehaviour {

    public Animator divingBellAnimator;
    public Text nameCaption;
    public Text ageCaption;
    public Text inputField;

    public TutorialArea tutorialArea;

    private int _action;
    private bool _started;

    public void Awake() {
        _action = 0;

        nameCaption.enabled = true;
        ageCaption.enabled = false;

        _started = false;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Hand || _started) return;

        if(_action == 0) {
            //save name
            ScoreTracker.PlayerName = inputField.text;

            //enter age
            inputField.text = "";
            nameCaption.enabled = false;
            ageCaption.enabled = true;

            //change keyboard layout
        } else {
            //clear screen
            inputField.text = "";
            nameCaption.enabled = false;
            ageCaption.enabled = false;

            //start tutorial
            tutorialArea.StartTutorial();

            //start diving bell animation
            divingBellAnimator.SetTrigger("LevelEnter");

            _started = true;
        }

        _action++;
    }
}
