using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    public int nextSceneIndex;

    private AsyncOperation _nextScene;

    private Collider _collider;

    public void Start() {
        //doesnt work with awake
        StartCoroutine(LoadYourAsyncScene());

        _collider = GetComponent<Collider>();
        SetStatus(false); //disable the collider
    }

    public void OnTriggerEnter(Collider other) {
        //enable scene loading
        if(other.gameObject.tag == Tags.Player) {
            _nextScene.allowSceneActivation = true;
        }
    }

    IEnumerator LoadYourAsyncScene() {
        _nextScene = SceneManager.LoadSceneAsync(nextSceneIndex);   

        // Wait until the asynchronous scene fully loads
        while (!_nextScene.isDone) {
            yield return null;
        }
    }

    public void SetStatus(bool status) {
        //set collider according to the status
        _collider.enabled = status;
    }
}
