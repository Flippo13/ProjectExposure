using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//objective information container
public class CompanionObjective : MonoBehaviour {

    public ObjectiveType objectiveType;
    public ObjectiveTask objectiveTask;
    public AudioClip instructionClip;
    public AudioClip reinforcementClip;
    public float reinforcementInterval;

    public int trashAmount;

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
