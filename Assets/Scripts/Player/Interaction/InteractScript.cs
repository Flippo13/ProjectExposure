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
        _collider = GetComponent<Collider>();
        _state = VacuumState.Companion;

        _rigidbody.useGravity = true;
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
        } else if(_state == VacuumState.Player && !grabber.IsGrabbing()) {
            SetVacuumState(VacuumState.Free);
        } else if(_state != VacuumState.Player && grabber.InVacuumMode() && grabber.IsGrabbing()) {
            SetVacuumState(VacuumState.Player);
        }
    }

    public void SetVacuumState(VacuumState state) {
        _state = state;

        Debug.Log("Entering Vaccum state: " + state);

        switch (state) {

            case VacuumState.Companion:
                //attach to companion and reset
                transform.parent = vacuumAnchor;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                _collider.isTrigger = true;
                _rigidbody.isKinematic = true;

                break;

            case VacuumState.Player:
                //release
                transform.parent = null;

                _collider.isTrigger = true;
                _rigidbody.isKinematic = true;

                break;

            case VacuumState.Free:
                //release
                transform.parent = null;

                _collider.isTrigger = false;
                _rigidbody.isKinematic = false;

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
            Rigidbody currentRigidbody = currentTransform.GetComponent<Rigidbody>();

            currentRigidbody.isKinematic = true;

            //translate to vacuum gun collider and scale down
            currentTransform.position += suckDir * suckSpeed;
            currentTransform.localScale = currentTransform.localScale * scaleFactor;

            //apply deformation
            float newDeform = Mathf.Clamp01(currentRenderer.material.GetFloat("_Deform") + deformationStep);
            currentRenderer.material.SetFloat("_Deform", newDeform);

            currentRigidbody.isKinematic = false;
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
