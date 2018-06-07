using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour {

    public Transform vacuumAnchor;
    public VacuumArea vacuumArea;
    public float suckSpeed;
    public ObjectGrabber grabber;
    public float deformationStep;
    public float scaleFactor;

    private int _trashCount;

    private List<Transform> _destroyedObjects;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private VacuumState _state;

    // Use this for initialization
    void Awake() {
        _destroyedObjects = new List<Transform>();
        _trashCount = 0;

        _rigidbody = GetComponent<Rigidbody>();
        _state = VacuumState.Companion;
    }

    public void OnTriggerEnter(Collider other) {
        if (_state != VacuumState.Player) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Suckable") && !_destroyedObjects.Contains(other.transform)) {
            _trashCount++;
            _destroyedObjects.Add(other.transform);
        }
    }

    public void Update() {
        //input and suck
        if(_state == VacuumState.Player && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5) {
            MoveTrash();
        } else if(_state == VacuumState.Player && !grabber.InVacuumMode()) {
            SetVacuumState(VacuumState.Free);
        } else if(_state != VacuumState.Player && grabber.InVacuumMode()) {
            SetVacuumState(VacuumState.Player);
        }
    }

    public void SetVacuumState(VacuumState state) {
        _state = state;

        switch (state) {

            case VacuumState.Companion:
                //attach to companion and reset
                transform.parent = vacuumAnchor;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;

                _collider.isTrigger = true;

                break;

            case VacuumState.Player:
                //release
                transform.parent = null;

                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;

                _collider.isTrigger = true;

                break;

            case VacuumState.Free:
                //release
                transform.parent = null;

                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;

                _collider.isTrigger = false;

                break;

            default:
                break;
        }
    }

    public VacuumState GetVacuumState() {
        return _state;
    }

    public void MoveTrash() {
        if (vacuumArea.suckableObjectsList.Count == 0) return;

        //expensive loop (better: make a suckable object script and cache components that need to be accessed)
        for (int i = 0; i < vacuumArea.suckableObjectsList.Count; i++) {
            Vector3 suckDir = (transform.position - vacuumArea.suckableObjectsList[i].position).normalized;
            Transform currentTransform = vacuumArea.suckableObjectsList[i];
            Renderer currentRenderer = currentTransform.GetComponent<Renderer>();

            //translate to vacuum gun collider and scale down
            currentTransform.Translate(suckDir * suckSpeed);
            currentTransform.localScale = currentTransform.localScale * scaleFactor;

            //apply deformation
            float newDeform = Mathf.Clamp01(currentRenderer.material.GetFloat("_Deform") + deformationStep);
            currentRenderer.material.SetFloat("_Deform", newDeform);
        }

        //clean up the grabber and vacuum area lists, then destroy the object
        for (int i = 0; i < _destroyedObjects.Count; i++) {
            vacuumArea.RemoveTransfromFromList(_destroyedObjects[i]);
            grabber.RemoveGrabCandidate(_destroyedObjects[i]);
            Destroy(_destroyedObjects[i].gameObject);
        }

        _destroyedObjects.Clear();
    }

    public int GetTrashCount() {
        return _trashCount;
    }
}
