using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabber : OVRGrabber {

    public bool isVacuumGrabber;
    public Transform vacuumAnchor;

    private bool _vacuumMode;
    private bool _grabbing;

    void OnTriggerEnter(Collider otherCollider) {
        if (otherCollider.tag == Tags.Vacuum) {
            if (isVacuumGrabber) _vacuumMode = true;
            else return; //dont allow grabbing vacuum
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
            return; //dont allow grabbing vacuum
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

    public bool IsGrabbing() {
        return _grabbing;
    }

    protected override void CheckForGrabOrRelease(float prevFlex) {
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin)) {
            GrabBegin();
            _grabbing = true;
        } else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd)) {
            GrabEnd();
            _grabbing = false;

            if (isVacuumGrabber) _vacuumMode = false;
        }
    }

    protected override void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false) {
        if (m_grabbedObj == null) {
            return;
        }

        if(_vacuumMode) {
            //handle vacuum repositioning (world position and rotation)
            m_grabbedObj.transform.position = vacuumAnchor.position;
            m_grabbedObj.transform.rotation = vacuumAnchor.rotation;

        } else {
            //default
            Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
            Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
            Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

            if (forceTeleport) {
                grabbedRigidbody.transform.position = grabbablePosition;
                grabbedRigidbody.transform.rotation = grabbableRotation;
            } else {
                grabbedRigidbody.MovePosition(grabbablePosition);
                grabbedRigidbody.MoveRotation(grabbableRotation);
            }
        }
    }

    public void InterruptGrabbing() {
        //end grabbing manually
        GrabEnd();
    }

    public void RemoveGrabCandidate(Transform grabbableTransform) {
        OVRGrabbable grabbable = grabbableTransform.GetComponent<OVRGrabbable>();

        if (grabbable == null) return;

        if (m_grabCandidates.ContainsKey(grabbable)) m_grabCandidates.Remove(grabbable);
    }
}
