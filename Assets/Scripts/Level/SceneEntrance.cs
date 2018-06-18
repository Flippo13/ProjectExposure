using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntrance : MonoBehaviour {

    public Transform player;
    public Transform divingBell;
    public bool isBeginning;
    public SceneTransition sceneTransition;

    private bool _completedBegin;

    [SerializeField]
    [FMODUnity.EventRef]
    private string _doorOpen;

    [SerializeField]
    [FMODUnity.EventRef]
    private string _doorClose;

    public void Awake() {
        _completedBegin = false;
    }

    public void SetParentStatus(string status) {
        if(status == "true") {
            if(isBeginning) {
                //parent to the diving bell
                player.parent = divingBell;
            } else if(!isBeginning && _completedBegin) {
                sceneTransition.EnableTransition(); //go to next level
            }
            
        } else if (status == "false") {
            if(isBeginning) {
                //unparent
                player.parent = null;

                _completedBegin = true;
            } else if (!isBeginning && _completedBegin) {
                player.parent = divingBell;
            }
            
        }
    }

    public void DoorSound(string open)
    {
        if (open == "true")
        {
            FMODUnity.RuntimeManager.PlayOneShot(_doorOpen, transform.position);
        }
        else
            FMODUnity.RuntimeManager.PlayOneShot(_doorClose, transform.position);

    }
}
