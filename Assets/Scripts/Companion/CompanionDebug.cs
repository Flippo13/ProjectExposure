using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//helpoer debug component for the companion
public class CompanionDebug : MonoBehaviour {

    public Material inactiveMaterial;
    public Material activeMaterial;
    public Material waitingMaterial;
    public Material busyMaterial;
    public Material gunMaterial;

    private Renderer _renderer;

    public void Init() {
        if (XRDevice.model != "") Debug.Log(XRDevice.model + " loaded");

        _renderer = GetComponent<Renderer>();
    }

    public void ApplyState(CompanionState state) {
        if (_renderer == null || !_renderer.enabled) return;

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

            case CompanionState.Waiting:
                _renderer.material = waitingMaterial;
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
        if(_renderer != null) _renderer.enabled = status;
    }
}
