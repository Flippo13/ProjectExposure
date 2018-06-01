using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionModel : MonoBehaviour {

    public Transform robotModel;
    public Transform transformationModel;
    public Transform vacuumModel;

    private Vector3 _storedScale;

    public void Awake() {
        ActivateRobot();

        _storedScale = transformationModel.localScale;
    }

    public void ActivateRobot() {
        robotModel.gameObject.SetActive(true);
        transformationModel.localScale = new Vector3(0, 0, 0);
        vacuumModel.gameObject.SetActive(false);
    }

    public void ActivateTransformation() {
        robotModel.gameObject.SetActive(false);
        transformationModel.localScale = _storedScale;
        vacuumModel.gameObject.SetActive(false);
    }

    public void ActivateVacuum() {
        robotModel.gameObject.SetActive(false);
        transformationModel.localScale = new Vector3(0, 0, 0);
        vacuumModel.gameObject.SetActive(true);
    }
}
