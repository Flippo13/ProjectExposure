using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackSliderHandler : MonoBehaviour {

    public Slider feedbackSlider;

	public void SetValue(int value) {
        feedbackSlider.value = value;

        if (gameObject.name == "FeedbackSlider1") ScoreTracker.Feedback1 = (int)feedbackSlider.value;
        else ScoreTracker.Feedback2 = (int)feedbackSlider.value;

    }
}
