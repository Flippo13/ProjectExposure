﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFade : OVRScreenFade {

    public float maxAlpha;

    private bool _secondFade;

    public void StartTeleportFade() {
        _secondFade = false;
        StartCoroutine(FadeTeleport(0, maxAlpha)); //first fade out
    }

    IEnumerator FadeTeleport(float startAlpha, float endAlpha) {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            currentAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTime));
            SetMaterialAlpha();

            if(elapsedTime >= fadeTime && !_secondFade) {
                _secondFade = true;
                StartCoroutine(FadeTeleport(maxAlpha, 0)); //fade in when done with fading out
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
