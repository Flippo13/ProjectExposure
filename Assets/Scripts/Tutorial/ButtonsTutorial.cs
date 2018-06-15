using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsTutorial : MonoBehaviour
{

    [Header("Left Hand")]

    [SerializeField]
    private GameObject _leftOculusController;

    [SerializeField]
    private GameObject _teleportButton;

    [SerializeField]
    private GameObject _tutorialButton;

    [SerializeField]
    private GameObject _leftGrabButton;

    [Header("Right Hand")]
    [SerializeField]
    private GameObject _rightOculusController;

    [SerializeField]
    private GameObject _rightGrabButton;

    [SerializeField]
    private GameObject _vacuumButton;
    [SerializeField]
    private GameObject _callButton;

    private bool _tutorialActive;
    private bool _fullTutorialActive;

    public void SetFullTutorial(bool value)
    {
        SetController(value, false);
        SetController(value, true);

        _teleportButton.SetActive(value);
        _tutorialButton.SetActive(value);
        _leftGrabButton.SetActive(value);
        _rightGrabButton.SetActive(value);
        _vacuumButton.SetActive(value);
        _callButton.SetActive(value);
    }

    public void SetButtonTutorial(TutorialButtons button, bool value)
    {
        switch (button)
        {
            case TutorialButtons.Teleport:
                _teleportButton.SetActive(value);
                SetController(value, false);
                break;
            case TutorialButtons.LeftGrab:
                _leftGrabButton.SetActive(value);
                SetController(value, false);
                break;
            case TutorialButtons.RightGrab:
                _rightGrabButton.SetActive(value);
                SetController(value, true);
                break;
            case TutorialButtons.Vacuum:
                _vacuumButton.SetActive(value);
                SetController(value, true);
                break;
            case TutorialButtons.CallCompanion:
                _callButton.SetActive(value);
                SetController(value, true);
                break;
            case TutorialButtons.FullTutorial:
                _tutorialButton.SetActive(value);
                SetController(value, false);
                break;
        }
    }

    private void SetController(bool value, bool rightHand)
    {
        if (rightHand)
        {
            _rightOculusController.SetActive(value);
            if (value)
            {
                _rightOculusController.GetComponent<Animator>().Play("ControllerFadeR_in");
            }
            else
                _rightOculusController.GetComponent<Animator>().Play("ControllerFadeR_out");

        }
        else
        {
            _leftOculusController.SetActive(value);
            if (value)
            {
                _leftOculusController.GetComponent<Animator>().Play("ControllerFadeL_in");
            }
            else
                _leftOculusController.GetComponent<Animator>().Play("ControllerFadeL_out");
        }
    }



    private void FixedUpdate()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three) && !_fullTutorialActive)
        {
            _fullTutorialActive = true;
            SetFullTutorial(true);
        }
        else if (OVRInput.GetUp(OVRInput.Button.Three) && _fullTutorialActive)
        {
            _fullTutorialActive = false;
            SetFullTutorial(false);
        }

    }
}
