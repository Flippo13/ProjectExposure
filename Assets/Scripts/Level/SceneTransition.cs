using System.Collections;
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
    }

    public void Update() {
        //dirty
        if(ScoreTracker.CompletedTurbines >= 2) {
            divingbellAnimator.SetTrigger("LevelEnter");
        }

        if(divingbellAnimator.GetCurrentAnimatorStateInfo(0).IsName("Arrived")) {
            EnableTransition();
        }
    }

    public void OnTriggerStay(Collider other) {
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
}
