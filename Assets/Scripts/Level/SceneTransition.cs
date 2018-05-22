using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    public int sceneIndex;

    private AsyncOperation _nextScene;

    public void Awake() {
        StartCoroutine(LoadYourAsyncScene());
    }

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == Tags.Player) {
            _nextScene.allowSceneActivation = true;
        }
    }

    IEnumerator LoadYourAsyncScene() {
        _nextScene = SceneManager.LoadSceneAsync(sceneIndex);   

        // Wait until the asynchronous scene fully loads
        while (!_nextScene.isDone) {
            yield return null;
        }
    }
}
