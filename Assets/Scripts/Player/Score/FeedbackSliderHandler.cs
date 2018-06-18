using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackSliderHandler : MonoBehaviour {

    private Slider _feedbackSlider;

    public void Awake() {
        _feedbackSlider = GetComponent<Slider>();
    }

    public void SetValue(int value) {
        _feedbackSlider.value = value;

        if (gameObject.name == "FeedbackSlider1") ScoreTracker.Feedback1 = (int)_feedbackSlider.value;
        else ScoreTracker.Feedback2 = (int)_feedbackSlider.value;

    }
}
