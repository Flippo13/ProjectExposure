using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionModel : MonoBehaviour {

    public Transform robotModel;
    public Transform vacuumModel;

    public void Awake() {
        ActivateRobot();
    }

    public void ActivateRobot() {
        robotModel.gameObject.SetActive(true);
        vacuumModel.gameObject.SetActive(false);
    }

    public void ActivateVacuum() {
        robotModel.gameObject.SetActive(false);
        vacuumModel.gameObject.SetActive(true);
    }
}
