using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour {

    public VacuumArea vacuumArea;
    public float suckSpeed;
    public CompanionGrabber grabber;
    public float deformationStep;
    public float scaleFactor;

    private int _trashCount;

    private List<Transform> _destroyedObjects;

    // Use this for initialization
    void Awake() {
        _destroyedObjects = new List<Transform>();
        _trashCount = 0;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Suckable") && !_destroyedObjects.Contains(other.transform)) {
            _trashCount++;
            _destroyedObjects.Add(other.transform);
        }
    }

    public void Suck() {
        if (vacuumArea.suckableObjectsList.Count == 0) return;

        //expensive loop (better: make a suckable object script and cache components that need to be accessed)
        for (int i = 0; i < vacuumArea.suckableObjectsList.Count; i++) {
            Vector3 suckDir = (transform.position - vacuumArea.suckableObjectsList[i].position).normalized;
            Transform currentTransform = vacuumArea.suckableObjectsList[i];
            Renderer currentRenderer = currentTransform.GetComponent<Renderer>();
            Rigidbody currentRigidbody = currentTransform.GetComponent<Rigidbody>();

            currentRigidbody.isKinematic = true;
            currentRigidbody.useGravity = false;

            //translate to vacuum gun collider and scale down
            currentTransform.Translate(suckDir * suckSpeed);
            currentTransform.localScale = currentTransform.localScale * scaleFactor;

            //apply deformation
            float newDeform = Mathf.Clamp01(currentRenderer.material.GetFloat("_Deform") + deformationStep);
            currentRenderer.material.SetFloat("_Deform", newDeform);

            //potential fix
            currentRigidbody.isKinematic = false;
            currentRigidbody.useGravity = true;
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
