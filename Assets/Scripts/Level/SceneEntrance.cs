using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntrance : MonoBehaviour {

    public Transform player;
    public Transform divingBell;
    public bool isBeginning;
    public SceneTransition sceneTransition;

    private bool _completedBegin;

    private Animator _animator;

    public void Awake() {
        _completedBegin = false;

        _animator = GetComponent<Animator>();
    }

    public void Update() {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Done") && isBeginning) {
            gameObject.SetActive(false); //disable when animation is done
        }

        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Move_down") && !isBeginning) {
            divingBell.gameObject.SetActive(true);
        }
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
}
