using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {
    public Text fpsCount;

    private float _updateTimer;
    private float _updateInterval;

    public void Awake() {
        _updateInterval = 1f / 3f;
    }

    public void Update() {
        if(_updateTimer >= _updateInterval) {
            fpsCount.text = "FPS: " + Mathf.Round(1f / Time.deltaTime);
            _updateTimer = 0f;
        }

        _updateTimer += Time.deltaTime;
    }
}