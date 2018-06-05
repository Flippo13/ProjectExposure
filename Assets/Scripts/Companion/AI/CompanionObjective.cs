using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

//objective information container
public class CompanionObjective : MonoBehaviour {

    //default information
    public ObjectiveType objectiveType;
    public ObjectiveTask objectiveTask;

    [EventRef]
    public string instructionClip;

    [EventRef]
    public string reinforcementClip;

    public float reinforcementInterval;

    //task dependant information
    public int trashAmount;
    public SceneTransition sceneTransition;

    private ObjectiveStatus _status;

    public void Awake() {
        _status = ObjectiveStatus.Incomplete;
    }

    public void SetStatus(ObjectiveStatus status) {
        _status = status;
    }

    public bool IsCompleted() {
        return _status == ObjectiveStatus.Complete;
    }

    public bool IsActive() {
        return _status == ObjectiveStatus.Active;
    }
}
