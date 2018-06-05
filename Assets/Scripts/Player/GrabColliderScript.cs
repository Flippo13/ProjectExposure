using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabColliderScript : MonoBehaviour {

    private bool _inCollider;

    public void Awake() {
        _inCollider = false;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != Tags.Companion) return;
        Debug.Log("Companion entered hand collider");
        _inCollider = true;
    }

    public void OnTriggerExit(Collider other) {
        if (other.gameObject.tag != Tags.Companion) return;
        Debug.Log("Companion exited hand collider");
        _inCollider = false;
    }

    public bool InCollider() {
        return _inCollider;
    }
}
