using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    public Transform companionDestination;
    public InteractScript vacuum;

    public float interactionRadius;
    public float objectiveScanRadius;

    public bool debug;

    private CompanionState _aiState;

    private CompanionControls _controls;
    private CompanionNavigator _navigator;
    private CompanionAudio _audio;
    private CompanionAnimation _animation;
    private CompanionObjectiveTracker _tracker;
    private CompanionDebug _debug;

    private float _timer;

    public void Awake() {
        //get all relevant components
        _controls = GetComponent<CompanionControls>();
        _navigator = GetComponent<CompanionNavigator>();
        _audio = GetComponent<CompanionAudio>();
        _animation = GetComponent<CompanionAnimation>();
        _tracker = GetComponent<CompanionObjectiveTracker>();
        _debug = GetComponent<CompanionDebug>();

        //if first boat scene: Inactive, otherwise: Following
        _debug.Init();
        _debug.SetRendererStatus(debug);
        EnterState(CompanionState.Following);
    }

    public void Update() {
        UpdateTracker();
        UpdateState();
    }

    private void SetState(CompanionState newState) {
        ExitState(_aiState);
        EnterState(newState);
    }

    private bool InInterationRange() {
        Vector3 deltaVec = companionDestination.transform.position - transform.position;

        return deltaVec.magnitude <= interactionRadius; //returns true, if the companion is in the interaction range of the player
    }

    private bool CheckForCompanionCall() {
        if (_controls.CallButtonDown() || Input.GetKeyDown(KeyCode.Q)) {
            //call the companion
            SetState(CompanionState.Returning);
            return true;
        }

        return false;
    }

    //returns true, if an objective was found
    private bool CheckForObjectives() {
        if (_tracker.GetCurrentObjective() != null && _tracker.GetCurrentObjective().IsActive()) return false; 

        CompanionObjective mainObjective = _tracker.GetNextMainObjective();
        CompanionObjective sideObjective = _tracker.GetClosestSideObjective();

        if (mainObjective != null && sideObjective != null) {   //main and side remaining
            float mainDistance = _tracker.GetObjectiveDistance(mainObjective);
            float sideDistance = _tracker.GetObjectiveDistance(sideObjective);

            float closest = Mathf.Min(mainDistance, sideDistance);

            if (closest == mainDistance && mainDistance < objectiveScanRadius) {
                //if it is closer than the side objective and in scan radius
                _tracker.SetCurrentObjective(mainObjective);
                SetState(CompanionState.Traveling);

                return true;
            } else if (closest == sideDistance && sideDistance < objectiveScanRadius) {
                //if it is closer than the main objective and in scan radius
                _tracker.SetCurrentObjective(sideObjective);
                SetState(CompanionState.Roaming);

                return true;
            }
        } else if (mainObjective == null && sideObjective != null) { //only side objectives remaining
            float sideDistance = _tracker.GetObjectiveDistance(sideObjective);

            if (sideDistance < objectiveScanRadius) {
                _tracker.SetCurrentObjective(sideObjective);
                SetState(CompanionState.Roaming);

                return true;
            }

        } else if (sideObjective == null && mainObjective != null) { //only main objectives remaining
            float mainDistance = _tracker.GetObjectiveDistance(mainObjective);

            if (mainDistance < objectiveScanRadius) {
                _tracker.SetCurrentObjective(mainObjective);
                SetState(CompanionState.Traveling);

                return true;
            }
        } else { //no objective remaining
            Debug.Log("No Main or Side Objectives found");
        }

        //nothing in scan radius
        return false;
    }

    private void UpdateTracker() {
        if (_tracker.GetCurrentObjective() != null && _tracker.GetCurrentObjective().IsActive()) {
            //track progress
            if (!_tracker.TrackObjective(_controls.GetTrashCount())) {
                //if the objective is completed
                _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Complete);
                Debug.Log("Objective complete");
            }
        }
    }

    private void EnterState(CompanionState state) {
        Debug.Log("Entering state " + state);

        switch (state) {

            case CompanionState.Traveling:
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Roaming:
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Waiting:
                _timer = float.MaxValue; //ensure to play the reinforcement once at the start

                break;

            case CompanionState.Instructing:
                _audio.StopAudioSource(AudioSourceType.Voice);
                _audio.SetClip(_tracker.GetCurrentObjective().instructionClip, AudioSourceType.Voice);
                _audio.PlayAudioSource(AudioSourceType.Voice);

                break;

            default:
                break;
        }

        _aiState = state;
        _debug.ApplyState(_aiState);
    }

    private void ExitState(CompanionState state) {
        Debug.Log("Leaving state " + state);

        switch (state) {
            default:
                break;
        }
    }

    //maybe split this up into different functions or even classes if it becomes to much
    private void UpdateState() {

        switch (_aiState) {
            case CompanionState.Following:
                //idle/main state of the companion
                if (CheckForCompanionCall()) return;

                //check wether the vacuum was dropped or not
                if (vacuum.GetVacuumState() == VacuumState.Free) {
                    SetState(CompanionState.GettingVacuum);
                }

                if (!CheckForObjectives() && !InInterationRange()) { //if there is no objective in range and the player is out of range
                    //move to the player
                    Vector3 deltaVecPlayer = transform.position - companionDestination.transform.position;
                    Vector3 destination = companionDestination.transform.position + deltaVecPlayer.normalized * interactionRadius; //player pos plus an offset

                    _navigator.SetDestination(destination);
                }

                break;

            case CompanionState.Returning:
                if (!InInterationRange()) {
                    //move to the player without other priorities
                    Vector3 deltaVecPlayer = transform.position - companionDestination.transform.position;
                    Vector3 destination = companionDestination.transform.position + deltaVecPlayer.normalized * interactionRadius;

                    _navigator.SetDestination(destination);

                } else {
                    //close enough to the player
                    SetState(CompanionState.Following);
                }

                break;

            case CompanionState.Traveling:
                //travel to main objective
                if (CheckForCompanionCall()) return;

                if (_navigator.ReachedDestinaton()) SetState(CompanionState.Waiting);

                break;

            case CompanionState.Roaming:
                //roam to side objective
                if (CheckForCompanionCall()) return;

                if (_navigator.ReachedDestinaton()) SetState(CompanionState.Waiting);

                break;

            case CompanionState.Waiting:
                if (CheckForCompanionCall()) return;

                if (InInterationRange()) {
                    //if the player is close enough, start instructing
                    SetState(CompanionState.Instructing);
                } else if (_timer >= _tracker.GetCurrentObjective().reinforcementInterval) {
                    //wait and reinforce the player for objective
                    _timer = 0f;
                    _audio.StopAudioSource(AudioSourceType.Voice);
                    _audio.SetClip(_tracker.GetCurrentObjective().reinforcementClip, AudioSourceType.Voice);
                    _audio.PlayAudioSource(AudioSourceType.Voice);
                }

                _timer += Time.deltaTime;

                break;

            case CompanionState.Instructing:
                //instruct the player about objective

                if (_audio.IsPlaying(AudioSourceType.Voice) == FMOD.Studio.PLAYBACK_STATE.STOPPED) {
                    //instructions are done, so either start the objective, reinforce the objective or follow

                    //activate the current task and go back to the following state
                    _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Active);
                    _tracker.StartTracking(_controls.GetTrashCount());
                    SetState(CompanionState.Following);
                }

                break;

            case CompanionState.GettingVacuum:
                //pick up or catch the vacuum gun

                Vector3 vacuumPos = vacuum.transform.position;
                Vector3 deltaVecVacuum = vacuumPos - transform.position;

                _navigator.SetDestination(vacuumPos);

                if (deltaVecVacuum.magnitude <= 0.5f) {
                    vacuum.SetVacuumState(VacuumState.Companion);
                    SetState(CompanionState.Following);
                }

                break;

            default:
                break;
        }
    }

}