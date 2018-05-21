using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//objective information container
public class CompanionObjective : MonoBehaviour {

    public CompanionObjectiveType objectiveType;

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
