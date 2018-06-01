using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour {

    public VacuumArea vacuumArea;
    public float suckSpeed;
    public CompanionGrabber grabber;

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
        for (int i = 0; i < vacuumArea.suckableObjectsList.Count; i++) {
            Vector3 suckDir = (transform.position - vacuumArea.suckableObjectsList[i].position).normalized;
            vacuumArea.suckableObjectsList[i].Translate(suckDir * suckSpeed);
        }

        for(int i = 0; i < _destroyedObjects.Count; i++) {
            vacuumArea.RemoveTransfromFromList(_destroyedObjects[i]);
            grabber.RemoveGrabCandidate(_destroyedObjects[i]);
            Destroy(_destroyedObjects[i].gameObject);
            //_destroyedObjects[i].gameObject.SetActive(false); //try to disable instead of destroying the object
        }

        _destroyedObjects.Clear();
    }

    public int GetTrashCount() {
        return _trashCount;
    }
}
