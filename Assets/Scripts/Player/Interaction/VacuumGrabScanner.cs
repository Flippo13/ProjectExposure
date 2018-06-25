using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumGrabScanner : MonoBehaviour {

    private bool _reachesForVacuum;

    public void Awake() {
        _reachesForVacuum = false;
    }

    public void OnTriggerEnter(Collider other) {
        if(other.tag == Tags.Hand && other.GetComponent<ObjectGrabber>().isVacuumGrabber) {
            _reachesForVacuum = true;
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.tag == Tags.Hand && other.GetComponent<ObjectGrabber>().isVacuumGrabber) {
            _reachesForVacuum = false;
        }
    }

    public bool IsReachingForVacuum() {
        return _reachesForVacuum;
    }
}
