using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionVacuum : MonoBehaviour {

    public InteractScript vacuumInteract;

    private Transform _vacuumTransform;

    public void Awake() {
        _vacuumTransform = vacuumInteract.transform;
    }

    public void SetVacuumState(VacuumState state) {
        vacuumInteract.SetVacuumState(state);
    }

    public VacuumState GetVacuumState() {
        return vacuumInteract.GetState();
    }

    public Transform GetVacuumTransform() {
        return _vacuumTransform;
    }
}
