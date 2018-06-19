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
    [SerializeField]
    private bool _lerpLeft;
    [SerializeField]
    private bool _lerping;

    [Header("Hand meshes")]
    [SerializeField]
    private GameObject _leftHand;
    [SerializeField]
    private GameObject _rightHand;

    private Material _leftMaterial;
    private Material _rightMaterial;
    private float lerpValue = 0f;
    private bool _fade;
    private bool _bothHands;

    private void Awake()
    {
        _leftMaterial = _leftHand.GetComponent<Renderer>().material;
        _rightMaterial = _rightHand.GetComponent<Renderer>().material;
    }
    public void SetFullTutorial(bool active)
    {
        SetController(active, false);
        SetController(active, true);
        _bothHands = true;
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

            default:
                break;
        }
    }

    public void SetController(bool active, bool rightHand)
    {
        if (rightHand)
        {
            _bothHands = false;
            _rightOculusController.SetActive(active);
            _lerpLeft = false;
        }

        else
        {
            _leftOculusController.SetActive(active);
            _lerpLeft = true;
            _bothHands = true;
        }
        if (_bothHands)
        {
            _leftOculusController.SetActive(active);
            _rightOculusController.SetActive(active);
        }

        if (active)
        {
            lerpValue = 0;
            _lerping = true;
        }
        else
        {
            lerpValue = 1;
            _lerping = false;
        }

        _fade = true;
    }


    private void Update()
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


        if (Input.GetKeyDown(KeyCode.E) && !_fullTutorialActive)
        {
            _fullTutorialActive = true;
            SetFullTutorial(true);
        }
        else if (Input.GetKeyUp(KeyCode.E) && _fullTutorialActive)
        {
            _fullTutorialActive = false;
            SetFullTutorial(false);
        }

        if (!_fade) return;

        if (_lerping && lerpValue < 1f)
        {
            lerpValue += Time.deltaTime * 1.3f;
            lerpValue = Mathf.Clamp01(lerpValue);
            if (_lerpLeft)
            {
                _leftMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);
            }
            else
            {
                _rightMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);

            }
            if (_bothHands)
            {
                _leftMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);
                _rightMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);
            }

            if (lerpValue == 1)
            {
                _fade = false;
            }
        }
        else if (!_lerping && lerpValue > 0f)
        {
            lerpValue -= Time.deltaTime;
            lerpValue = Mathf.Clamp01(lerpValue);
            if (_lerpLeft)
            {
                _leftMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);
            }
            else
            {
                _rightMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);

            }

            if (_bothHands)
            {
                _leftMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);
                _rightMaterial.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.15f), lerpValue);
            }

            if (lerpValue == 0)
            {
                _fade = false;
            }
        }

    }
}
