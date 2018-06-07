using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabber : OVRGrabber {

    private bool _vacuumMode;
    private bool _grabbing;

    public void Update() {
        if(OVRInput.GetDown(OVRInput.Button.Three) || Input.GetKeyDown(KeyCode.I)) {
            //debug testing grab interrupting when pressing X
            InterruptGrabbing();
            Debug.Log("Interrupted grabbing");
        }
    }

    void OnTriggerEnter(Collider otherCollider) {
        if (otherCollider.tag == Tags.Vacuum) {
            _vacuumMode = true;
        }

        // Get the grab trigger
        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null || !grabbable.enabled) return;

        // Add the grabbable
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;
    }

    void OnTriggerExit(Collider otherCollider) {
        if (otherCollider.tag == Tags.Vacuum) {
            _vacuumMode = false;
        }

        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null || !grabbable.enabled) return;

        // Remove the grabbable
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found) {
            return;
        }

        if (refCount > 1) {
            m_grabCandidates[grabbable] = refCount - 1;
        } else {
            m_grabCandidates.Remove(grabbable);
        }
    }

    public bool InVacuumMode() {
        return _vacuumMode && _grabbing;
    }

    protected override void CheckForGrabOrRelease(float prevFlex) {
        if(_vacuumMode) {
            if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin)) {
                GrabBegin();
                _grabbing = true;
            } else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd)) {
                GrabEnd();
                _grabbing = false;
            }
        } else {
            base.CheckForGrabOrRelease(prevFlex);
        }
    }

    public void InterruptGrabbing() {
        //end grabbing manually
        GrabEnd();
        _grabbing = false;
    }

    public void RemoveGrabCandidate(Transform grabbableTransform) {
        OVRGrabbable grabbable = grabbableTransform.GetComponent<OVRGrabbable>();

        if (grabbable == null) return;

        if (m_grabCandidates.ContainsKey(grabbable)) m_grabCandidates.Remove(grabbable);
    }
}
