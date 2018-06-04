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

        for (int i = 0; i < vacuumArea.suckableObjectsList.Count; i++) {
            Vector3 suckDir = (transform.position - vacuumArea.suckableObjectsList[i].position).normalized;
            Transform currentTransform = vacuumArea.suckableObjectsList[i];
            Renderer currentRenderer = currentTransform.GetComponent<Renderer>();

            currentTransform.Translate(suckDir * suckSpeed);
            currentTransform.localScale = currentTransform.localScale * scaleFactor;

            float newDeform = Mathf.Clamp01(currentRenderer.material.GetFloat("_Deform") + deformationStep);
            currentRenderer.material.SetFloat("_Deform", newDeform);
        }

        for(int i = 0; i < _destroyedObjects.Count; i++) {
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
