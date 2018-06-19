using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDetector : MonoBehaviour {

    private bool _inBoundary;

    public void Awake() {
        _inBoundary = false;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Boundary) return;

        _inBoundary = true;
    }

    public void OnTriggerExit(Collider other) {
        if (other.tag != Tags.Boundary) return;

        _inBoundary = false;
    }

    public bool InBoundary() {
        return _inBoundary;
    }
}
