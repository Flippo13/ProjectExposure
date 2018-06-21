using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePointer : MonoBehaviour {

    public SpriteRenderer arrowChild;

    private SpriteRenderer _spriteRenderer;
    private bool _triggered;

    public void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _triggered = false;
    }

    public void LateUpdate() {
        transform.LookAt(Camera.main.transform);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Player || _triggered) return;

        Disable();
    }

    public bool IsTriggered() {
        return _triggered;
    }

    public void ResetPointer() {
        _spriteRenderer.enabled = true;
        arrowChild.enabled = true;

        _triggered = false;
    }

    public void Disable() {
        //disable the objective zone
        _spriteRenderer.enabled = false;
        arrowChild.enabled = false;

        _triggered = true;
    }
}
