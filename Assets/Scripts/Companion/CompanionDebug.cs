using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//helpoer debug component for the companion
public class CompanionDebug : MonoBehaviour {

    public Material inactiveMaterial;
    public Material activeMaterial;
    public Material busyMaterial;
    public Material gunMaterial;

    private Renderer _renderer;

    public void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    public void ApplyState(CompanionState state) {
        if (!_renderer.enabled || _renderer == null) return;

        switch(state) {
            case CompanionState.Inactive:
                _renderer.material = inactiveMaterial;
                break;

            case CompanionState.Instructing:
                _renderer.material = busyMaterial;
                break;

            case CompanionState.Transforming:
                _renderer.material = busyMaterial;
                break;

            case CompanionState.Useable:
                _renderer.material = gunMaterial;
                break;

            default:
                _renderer.material = activeMaterial;
                break;
        }
    }

    public void SetRendererStatus(bool status) {
        _renderer.enabled = status;
    }
}
