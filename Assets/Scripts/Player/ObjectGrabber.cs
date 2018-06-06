using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabber : OVRGrabber {

    private bool _vacuumMode;

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
        return _vacuumMode;
    }

    public void RemoveGrabCandidate(Transform grabbableTransform) {
        OVRGrabbable grabbable = grabbableTransform.GetComponent<OVRGrabbable>();

        if (grabbable == null) return;

        if (m_grabCandidates.ContainsKey(grabbable)) m_grabCandidates.Remove(grabbable);
    }
}
