using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : MonoBehaviour {

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Hand) return;

        ResetStatics();
        ReloadLevel();
    }

    private void ReloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetStatics() {
        ScoreTracker.PlayerName = "";
        ScoreTracker.PlayerAge = "";
        ScoreTracker.CompletedTurbines = 0; //TODO
        ScoreTracker.Score = 0;
        ScoreTracker.Feedback1 = 0; //not answered
        ScoreTracker.Feedback2 = 0; //not answered
    }
}
