using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoneButtonHandler : MonoBehaviour {

    public Animator divingBellAnimator;
    public Text nameField;

    public TutorialArea tutorialArea;

    private bool _started;

    public void Awake() {
        _started = false;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Hand || _started) return;

        //save name
        ScoreTracker.PlayerName = nameField.text;

        tutorialArea.StartTutorial();

        //start diving bell animation
        divingBellAnimator.SetTrigger("LevelEnter");

        _started = true;
    }
}
