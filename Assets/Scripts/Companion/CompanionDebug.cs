using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionDebug : MonoBehaviour {

    public Material inactiveMaterial;
    public Material activeMaterial;
    public Material busyMaterial;

    private Renderer _renderer;

    public void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    public void ApplyState(CompanionState state) {
        switch(state) {
            case CompanionState.Inactive:
                _renderer.material = inactiveMaterial;
                break;

            case CompanionState.Instructing:
                _renderer.material = busyMaterial;
                break;

            default:
                _renderer.material = activeMaterial;
                break;
        }
    }
}
