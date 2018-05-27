using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//objective information container
public class CompanionObjective : MonoBehaviour {

    public ObjectiveType objectiveType;
    public AudioClip instructionClip;
    public AudioClip reinforcementClip;
    public float reinforcementInterval;

    private bool _completed;

    public void Awake() {
        _completed = false;
    }

    public void Complete() {
        _completed = true;
    }

    public bool IsCompleted() {
        return _completed;
    }
}
