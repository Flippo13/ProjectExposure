using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashPilePointer : MonoBehaviour {

    public GameObject pointerPrefab;
    public bool showPointer;

    private ObjectivePointer _objectivePointer;

    public void Awake() {
        GameObject pointerInstance= Instantiate(pointerPrefab);
        pointerInstance.transform.parent = transform;
        pointerInstance.transform.position = transform.position;

        _objectivePointer = pointerInstance.GetComponent<ObjectivePointer>();

        _objectivePointer.Disable(); //of by default
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag != Tags.Scanner || !showPointer) return;

        _objectivePointer.ResetPointer(); //enable when close
    }

    public void OnTriggerExit(Collider other) {
        if (other.tag != Tags.Scanner || !showPointer) return;

        _objectivePointer.Disable();
    }


}
