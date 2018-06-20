using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoneButtonHandler : MonoBehaviour {

    public Animator divingBellAnimator;
    public Text captionField;
    public Text nameField;

    public TutorialArea tutorialArea;

    private int _action;
    private bool _started;

    public void Awake() {
        _action = 0;

        _started = false;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Hand || _started) return;

        if(_action == 0) {
            //save name
            ScoreTracker.PlayerName = nameField.text;

            //enter age
            nameField.text = "";
            captionField.text = "Enter your age";

            //change keyboard layout
        } else {
            //start tutorial
            tutorialArea.StartTutorial();

            //start diving bell animation
            divingBellAnimator.SetTrigger("LevelEnter");

            _started = true;
        }

        _action++;
    }
}
