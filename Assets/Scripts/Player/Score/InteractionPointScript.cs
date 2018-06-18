using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPointScript : MonoBehaviour {

    public FeedbackSliderHandler sliderHandler;
    public int value;

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Hand) return;

        sliderHandler.SetValue(value);
    }
}
