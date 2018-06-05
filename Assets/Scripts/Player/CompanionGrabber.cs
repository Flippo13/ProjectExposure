using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionGrabber : OVRGrabber {

    private bool _companionMode = false;
    private bool _grabbingInput = false;

    public void SetCompanionStatus(bool status) {
        _companionMode = status;
    }

    void OnTriggerEnter(Collider otherCollider) {
        if (otherCollider.tag == Tags.Companion) _companionMode = true;
        else _companionMode = false;

        // Get the grab trigger
        OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null || !grabbable.enabled) return;

        // Add the grabbable
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;
    }

    void OnTriggerExit(Collider otherCollider) {
        if(otherCollider.tag == Tags.Companion) _companionMode = false;

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

    //is called in fixed update
    protected override void CheckForGrabOrRelease(float prevFlex) {
        if(_companionMode) {
            return;
            //different input mode for the companion
            if (m_prevFlex >= grabBegin && !_grabbingInput) {
                Debug.Log("Grabber grab pressed");
                GrabBegin();
                _grabbingInput = true;
            } else if (m_prevFlex <= grabEnd && _grabbingInput) {
                Debug.Log("Grabber grab released");
                GrabEnd();
                _grabbingInput = false;
            }
        } else {
            base.CheckForGrabOrRelease(prevFlex);
        }

    }

    public void BeginGrabbing() {
        GrabBegin();
    }

    public void StopGrabbing() {
        GrabEnd();
    }

    public void RemoveGrabCandidate(Transform grabbableTransform) {
        OVRGrabbable grabbable = grabbableTransform.GetComponent<OVRGrabbable>();

        if (grabbable == null) return;

        if (m_grabCandidates.ContainsKey(grabbable)) m_grabCandidates.Remove(grabbable);
    }
}
