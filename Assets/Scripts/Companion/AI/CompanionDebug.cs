using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//helpoer debug component for the companion
public class CompanionDebug : MonoBehaviour {

    public Material activeMaterial;
    public Material waitingMaterial;
    public Material busyMaterial;
    public Material stayingMaterial;
    public Material vacuumMaterial;
    public Material handingMaterial;

    private Renderer _renderer;

    public void Init() {
        if (XRDevice.model != "") Debug.Log(XRDevice.model + " loaded");

        _renderer = GetComponent<Renderer>();
    }

    public void ApplyState(CompanionState state) {
        if (_renderer == null || !_renderer.enabled) return;

        switch(state) {

            case CompanionState.Staying:
                _renderer.material = stayingMaterial;

                break;

            case CompanionState.Instructing:
                _renderer.material = busyMaterial;
                break;

            case CompanionState.Waiting:
                _renderer.material = waitingMaterial;
                break;

            case CompanionState.GettingVacuum:
                _renderer.material = vacuumMaterial;
                break;

            case CompanionState.HandingVacuum:
                _renderer.material = handingMaterial;
                break;

            default:
                _renderer.material = activeMaterial;
                break;
        }
    }

    public void SetRendererStatus(bool status) {
        if(_renderer != null) _renderer.enabled = status;
    }
}
