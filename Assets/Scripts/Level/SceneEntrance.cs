using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntrance : MonoBehaviour {

    public Transform player;
    public bool isBeginning;
    public SceneTransition sceneTransition;

    private TeleporterScript _teleport;

    public void Awake() {
        _teleport = player.GetComponent<TeleporterScript>();
    }

    public void SetParentStatus(string status) {
        if(status == "true") {
            if(isBeginning) {
                //parent to the diving bell
                player.parent = transform;
                _teleport.SetInTransition(true);
            } else {
                sceneTransition.EnableTransition(); //go to next level
            }
            
        } else if (status == "false") {
            if(isBeginning) {
                //unparent
                player.parent = null;
                _teleport.SetInTransition(false);
            } else {
                player.parent = transform;
                _teleport.SetInTransition(true);
            }
            
        }
    }
}
