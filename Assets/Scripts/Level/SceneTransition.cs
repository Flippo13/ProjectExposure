﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    public int nextSceneIndex;
    public Animator divingbellAnimator;

    public bool debug;

    private AsyncOperation _nextScene;

    private Collider _collider;

    public void Start() {
        //doesnt work with awake
        if(!debug) StartCoroutine(LoadYourAsyncScene());

        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void OnTriggerEnter(Collider other) {
        if(other.tag == Tags.Player && divingbellAnimator != null) {
            divingbellAnimator.SetTrigger("LevelExit");
        }
    }

    IEnumerator LoadYourAsyncScene() {
        _nextScene = SceneManager.LoadSceneAsync(nextSceneIndex);
        _nextScene.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (!_nextScene.isDone) {
            yield return null;
        }
    }

    public void EnableTransition() {
        //go to next scene
        if(!debug) _nextScene.allowSceneActivation = true;
    }

    public void EnableCollider() {
        _collider.enabled = true;
    }
}
