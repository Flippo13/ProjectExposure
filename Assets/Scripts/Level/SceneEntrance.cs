using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntrance : MonoBehaviour {

    public Transform player;
    public Transform divingBell;
    public bool isBeginning;
    public SceneTransition sceneTransition;

    private TeleporterScript _teleport;
    private bool _completedBegin;

    public void Awake() {
        _teleport = player.GetComponent<TeleporterScript>();
        _completedBegin = false;
    }

    public void SetParentStatus(string status) {
        if(status == "true") {
            if(isBeginning) {
                //parent to the diving bell
                player.parent = divingBell;
                _teleport.SetInTransition(true);
            } else if(!isBeginning && _completedBegin) {
                sceneTransition.EnableTransition(); //go to next level
            }
            
        } else if (status == "false") {
            if(isBeginning) {
                //unparent
                player.parent = null;
                _teleport.SetInTransition(false);

                _completedBegin = true;
            } else if (!isBeginning && _completedBegin) {
                player.parent = divingBell;
                _teleport.SetInTransition(true);
            }
            
        }
    }
}
