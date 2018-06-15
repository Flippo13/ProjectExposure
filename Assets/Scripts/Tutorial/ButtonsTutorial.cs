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

    public void SetFullTutorial(bool active)
    {
        SetController(active, false);
        SetController(active, true);

        _teleportButton.SetActive(active);
        _tutorialButton.SetActive(active);
        _leftGrabButton.SetActive(active);
        _rightGrabButton.SetActive(active);
        _vacuumButton.SetActive(active);
        _callButton.SetActive(active);
    }

    public void SetButtonTutorial(TutorialButtons button, bool active)
    {
        switch (button)
        {
            case TutorialButtons.Teleport:
                _teleportButton.SetActive(active);
                SetController(active, false);
                break;
            case TutorialButtons.LeftGrab:
                _leftGrabButton.SetActive(active);
                SetController(active, false);
                break;
            case TutorialButtons.RightGrab:
                _rightGrabButton.SetActive(active);
                SetController(active, true);
                break;
            case TutorialButtons.Vacuum:
                _vacuumButton.SetActive(active);
                SetController(active, true);
                break;
            case TutorialButtons.CallCompanion:
                _callButton.SetActive(active);
                SetController(active, true);
                break;
            case TutorialButtons.FullTutorial:
                _tutorialButton.SetActive(active);
                SetController(active, false);
                break;
        }
    }

    IEnumerator SetController(bool active, bool rightHand)
    {
        Animator _animator;
        if (rightHand)
        {
            _animator = _rightOculusController.GetComponent<Animator>();
            if (active)
            {
                _rightOculusController.SetActive(active);
                _animator.Play("ControllerFadeR_in");
            }
            else
            {
                _animator.Play("ControllerFadeR_out");
                // Wait for the animation to end before deactivating it
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
                _rightOculusController.SetActive(active);
            }


        }
        else
        {
            _animator = _leftOculusController.GetComponent<Animator>();
            if (active)
            {
                _leftOculusController.SetActive(active);
                _animator.Play("ControllerFadeL_in");
            }
            else
            {
                _animator.Play("ControllerFadeL_out");
                // Wait for the animation to end before deactivating it
                yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
                _leftOculusController.SetActive(active);
            }
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
