using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionModel : MonoBehaviour {

    public Transform robotModel;
    public Transform transformationModel;
    public Transform vacuumModel;

    public void Awake() {
        ActivateRobot();
    }

    public void ActivateRobot() {
        robotModel.gameObject.SetActive(true);
        transformationModel.gameObject.SetActive(false);
        vacuumModel.gameObject.SetActive(false);
    }

    public void ActivateTransformation() {
        robotModel.gameObject.SetActive(false);
        transformationModel.gameObject.SetActive(true);
        vacuumModel.gameObject.SetActive(false);
    }

    public void ActivateVacuum() {
        robotModel.gameObject.SetActive(false);
        transformationModel.gameObject.SetActive(false);
        vacuumModel.gameObject.SetActive(true);
    }
}
