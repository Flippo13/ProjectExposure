using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialWaypoint : MonoBehaviour {

    public GameObject pointerPrefab;
    public bool forceGoingToPointer;
    public TutorialButtons tutorialButton;

    [FMODUnity.EventRef]
    public string tutorialVoiceline;

    private bool _active;
    private TutorialArea _tutorialArea;
    private ObjectivePointer _objectivePointer;

    private UnityEvent _callEvent;
    private bool _teleported;

    public void Awake() {
        _active = false;
        _teleported = false;

        if(tutorialButton == TutorialButtons.CallCompanion) {
            _callEvent = new UnityEvent();
        }
    }

    public void Update() {
        if (!_active) return;

        //if button condition is fulfilled and player is in waypoint
        if(CheckForButtonPress() && (_objectivePointer == null || _objectivePointer.IsTriggered())) {
            _tutorialArea.ActivateNextWaypoint(); //this one gets disabled and the next one enabled
        }
    }

    //check if the desired tutorial was completed (most likely needs more interations)
    public bool CheckForButtonPress() {
        switch(tutorialButton) {
            case TutorialButtons.Teleport:
                if(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f) {
                    _teleported = true;
                }

                return _teleported;

            case TutorialButtons.RightGrab:
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f && _tutorialArea.vacuum.vacuumGrabber.IsGrabbing() && _tutorialArea.vacuum.vacuumGrabber.InVacuumMode()) {
                    return true;
                }

                break;

            case TutorialButtons.Vacuum:
                //if the trigger is pressed while the vacuum is in the player's hand
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && _tutorialArea.vacuum.GetVacuumState() == VacuumState.Player) {
                    return true;
                }

                break;

            case TutorialButtons.CallCompanion:
                //call companion while still remaining in the tutorial state
                if (OVRInput.GetDown(OVRInput.Button.One)) {
                    _callEvent.Invoke(); //should call the companion
                    return true;
                }

                break;

            case TutorialButtons.FullTutorial:
                //might bug out cause the full button tutorial will activate itself
                if (OVRInput.GetDown(OVRInput.Button.Three)) {
                    return true;
                }

                break;

            default:
                if(_tutorialArea.companionAudio.GetStartedPlaying() && _tutorialArea.companionAudio.GetPlaybackState(AudioSourceType.Voice) == FMOD.Studio.PLAYBACK_STATE.STOPPED) {
                    //if the voiceline is over
                    return true;
                }
                break;
        }

        return false;
    }

    public void Deactivate() {
        _active = false;

        _tutorialArea.buttonsTutorial.SetButtonTutorial(tutorialButton, false); //disable tutorial

        if(_objectivePointer != null) _objectivePointer.Disable();
    }

    public void Activate(TutorialArea tutorialArea) {
        _active = true;

        _tutorialArea = tutorialArea; //set reference when activated
        _tutorialArea.buttonsTutorial.SetButtonTutorial(tutorialButton, true); //enable tutorial

        // set clip and play voiceline
        _tutorialArea.companionAudio.StopAudioSource(AudioSourceType.Voice);
        _tutorialArea.companionAudio.SetClip(tutorialVoiceline, AudioSourceType.Voice);
        StartCoroutine(_tutorialArea.companionAudio.PlayAudioSourceWithHaptic(AudioSourceType.Voice)); //apply vibration

        //acivate objective pointer for the current waypoint if possible
        _objectivePointer = null;

        if (tutorialButton != TutorialButtons.None && forceGoingToPointer) {
            GameObject pointerInstance = Instantiate(pointerPrefab);
            pointerInstance.transform.parent = transform;
            pointerInstance.transform.position = transform.position; //put pointer on waypoint position

            _objectivePointer = pointerInstance.GetComponent<ObjectivePointer>();
        }

        if(tutorialButton == TutorialButtons.CallCompanion) {
            CompanionAI ai = _tutorialArea.companionAudio.gameObject.GetComponent<CompanionAI>(); //quite dirty
            _callEvent.AddListener(ai.TutorialCall);
        }
    }
}
