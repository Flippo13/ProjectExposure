using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialWaypoint : MonoBehaviour {

    public GameObject pointerPrefab;
    public bool forceGoingToPointer;
    public TutorialButtons tutorialButton;
    public string animationTrigger;

    [FMODUnity.EventRef]
    public string tutorialVoiceline;

    private bool _active;
    private TutorialArea _tutorialArea;
    private ObjectivePointer _objectivePointer;

    private bool _teleported;

    public void Awake() {
        _active = false;
        _teleported = false;
    }

    public void Update() {
        if (!_active) return;

        //if button condition is fulfilled and player is in waypoint
        if(CheckForButtonPress() && (_objectivePointer == null || _objectivePointer.IsTriggered())) {
            Debug.Log("Activate  next waypoint called");
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

        Debug.Log("Deactivate button tutorial");

        if(_objectivePointer != null) _objectivePointer.Disable();
    }

    public void Activate(TutorialArea tutorialArea) {
        _active = true;

        _tutorialArea = tutorialArea; //set reference when activated
        _tutorialArea.buttonsTutorial.SetButtonTutorial(tutorialButton, true); //enable tutorial

        //register in tutorial area
        _tutorialArea.Activate(this);

        // set clip and play voiceline
        _tutorialArea.companionAudio.StopAudioSource(AudioSourceType.Voice);
        _tutorialArea.companionAudio.SetClip(tutorialVoiceline, AudioSourceType.Voice);
        StartCoroutine(_tutorialArea.companionAudio.PlayAudioSourceWithHaptic(AudioSourceType.Voice)); //apply vibration

        //play animation
        _tutorialArea.companionAudio.gameObject.GetComponent<CompanionAnimation>().SetAnimationTrigger(animationTrigger);

        //activate objective pointer for the current waypoint if possible
        _objectivePointer = null;

        if (tutorialButton != TutorialButtons.None && forceGoingToPointer) {
            GameObject pointerInstance = Instantiate(pointerPrefab);
            pointerInstance.transform.parent = transform;
            pointerInstance.transform.position = transform.position; //put pointer on waypoint position

            _objectivePointer = pointerInstance.GetComponent<ObjectivePointer>();
        }
    }

    public bool IsActive() {
        return _active;
    }
}
