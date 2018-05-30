using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    public Transform player;
    public Transform companionDestination;

    public float interactionRadius;
    public float grabRadius;
    public float transformationRadius;
    public float scanRadius;

    public bool debug;

    private CompanionState _aiState;
    private TransformationState _transformationState;
    private List<CompanionState> _stateQueue;

    private CompanionControls _controls;
    private CompanionNavigator _navigator;
    private CompanionModel _model;
    private CompanionAudio _audio;
    private CompanionAnimation _animation;
    private CompanionObjectiveTracker _tracker;
    private CompanionDebug _debug;

    private float _reinforcementTimer;

    public void Awake() {
        _stateQueue = new List<CompanionState>();

        _controls = GetComponent<CompanionControls>();
        _navigator = GetComponent<CompanionNavigator>();
        _model = GetComponent<CompanionModel>();
        _audio = GetComponent<CompanionAudio>();
        _animation = GetComponent<CompanionAnimation>();
        _tracker = GetComponent<CompanionObjectiveTracker>();
        _debug = GetComponent<CompanionDebug>();

        //if first boat scene: Inactive, otherwise: Following
        _debug.Init();
        _debug.SetRendererStatus(debug);
        EnterState(CompanionState.Following);
        _transformationState = TransformationState.None;
    }

    public void Update() {
        UpdateTracker();
        UpdateState();
    }

    private void SetState(CompanionState newState) {
        ExitState(_aiState);
        EnterState(newState);
    }

    private void QueueState(CompanionState nextState) {
        _stateQueue.Add(nextState);
    }

    private bool CheckQueueState() {
        if (_stateQueue.Count == 0) return false;

        SetState(_stateQueue[0]); //clean enter new state
        _stateQueue.RemoveAt(0); //remove front state of the queue

        return true;
    }

    private void ClearQueue() {
        _stateQueue.Clear();
    }

    private bool InInterationRange() {
        Vector3 deltaVec = player.transform.position - transform.position;

        return deltaVec.magnitude <= interactionRadius; //returns true, if the companion is in the interaction range of the player
    }

    private bool CheckForCompanionCall() {
        if (_controls.CallButtonDown()) {
            //call the companion and let it transform into the vacuum gun
            ClearQueue();
            QueueState(CompanionState.Useable);
            SetState(CompanionState.Transforming);

            return true;
        }

        return false;
    }

    private bool CheckForObjectives() {
        if (_tracker.GetCurrentObjective().IsActive()) return false; //if there is already an active quest, dont look for a new one

        CompanionObjective mainObjective = _tracker.GetNextMainObjective();
        CompanionObjective sideObjective = _tracker.GetClosestSideObjective();

        if(mainObjective != null && sideObjective != null) {
            float mainDistance = _tracker.GetObjectiveDistance(mainObjective);
            float sideDistance = _tracker.GetObjectiveDistance(sideObjective);

            float closest = Mathf.Min(mainDistance, sideDistance);

            if (closest == mainDistance && mainDistance < scanRadius) {
                //if it is closer than the side objective and in scan radius
                _tracker.SetCurrentObjective(mainObjective);
                SetState(CompanionState.Traveling);

                return true;
            } else if (closest == sideDistance && sideDistance < scanRadius) {
                //if it is closer than the main objective and in scan radius
                _tracker.SetCurrentObjective(sideObjective);
                SetState(CompanionState.Roaming);

                return true;
            }
        } else {
            if(mainObjective == null && sideObjective != null) {
                float sideDistance = _tracker.GetObjectiveDistance(sideObjective);

                if(sideDistance < scanRadius) {
                    _tracker.SetCurrentObjective(sideObjective);
                    SetState(CompanionState.Roaming);

                    return true;
                }
                
            } else if(sideObjective == null && mainObjective != null) {
                float mainDistance = _tracker.GetObjectiveDistance(mainObjective);

                if(mainDistance < scanRadius) {
                    _tracker.SetCurrentObjective(mainObjective);
                    SetState(CompanionState.Traveling);

                    return true;
                }
                
            }
        }

        //nothing in scan radius
        return false;
    }

    private void TransformToVacuum() {
        Vector3 deltaVec = companionDestination.transform.position - transform.position;

        if(deltaVec.magnitude <= transformationRadius) {
            //play transformation animation
        }

        if(deltaVec.magnitude > grabRadius) {
            //travel to the hand
            _navigator.SetSpeed(100f);
            _navigator.SetAcceleration(500f);
            _navigator.SetDestination(companionDestination.transform.position);
        } else {
            //destination reached
            _navigator.SetSpeed(3.5f);
            _navigator.SetAcceleration(8f);
            _transformationState = TransformationState.None;
        }
    }

    private void TransformToRobot() {
        _navigator.SetAgentStatus(false);

        //play transformation animation

        //when done
        _transformationState = TransformationState.None;
    }

    private void UpdateTracker() {
        if (_tracker.GetCurrentObjective() != null && _tracker.GetCurrentObjective().IsActive()) {
            //track progress
            if (!_tracker.TrackObjective(_controls.GetTrashCount())) {
                //if the objective is completed
                _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Complete);
                Debug.Log("Objective complete");
                CheckForObjectives(); //get next objective
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
                _reinforcementTimer = float.MaxValue;

                break;

            case CompanionState.Instructing:
                _audio.SetClip(_tracker.GetCurrentObjective().instructionClip, AudioSourceType.Voice);
                _audio.PlayAudioSource(AudioSourceType.Voice);

                break;

            case CompanionState.Transforming:
                //preparing the correct transformation
                if(_stateQueue.Count > 0) {
                    if(_stateQueue[0] == CompanionState.Useable) {
                        _transformationState = TransformationState.Vacuum;
                    } else if(_stateQueue[0] == CompanionState.Following) {
                        _transformationState = TransformationState.Robot;
                    }
                }

                break;

            case CompanionState.Grabbed:
                _debug.SetRendererStatus(false);
                _navigator.SetAgentStatus(false);
                _model.ActivateVacuum();
                
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
            case CompanionState.Grabbed:
                _debug.SetRendererStatus(debug);
                _navigator.SetAgentStatus(true);
                _model.ActivateRobot();

                break;

            default:
                break;
        }
    }

    //maybe split this up into different functions or even classes if it becomes to much
    private void UpdateState() {

        switch(_aiState) {
            case CompanionState.Inactive:
                //activate the companion
                if (_controls.CallButtonDown() && InInterationRange()) {
                    SetState(CompanionState.Following);
                }
                break;

            case CompanionState.Following:
                //idle/main state of the companion

                if (CheckForCompanionCall()) return; //prioritise calling

                if (!CheckForObjectives() && !InInterationRange()) { //if there is no objective in range and the player is out of range
                    //move to the player
                    Vector3 deltaVec = transform.position - player.transform.position;
                    Vector3 destination = player.transform.position + deltaVec.normalized * (interactionRadius - 1f / interactionRadius);

                    _navigator.SetDestination(destination);
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
                
                if(InInterationRange()) {
                    //if the player is close enough, start instructing
                    SetState(CompanionState.Instructing);
                    return;
                } else if(_reinforcementTimer >= _tracker.GetCurrentObjective().reinforcementInterval) { //maybe cache the current objective
                    //wait and reinforce the player for objective
                    _reinforcementTimer = 0f;
                    _audio.SetClip(_tracker.GetCurrentObjective().reinforcementClip, AudioSourceType.Voice);
                    _audio.PlayAudioSource(AudioSourceType.Voice);
                }

                _reinforcementTimer += Time.deltaTime;

                break;

            case CompanionState.Instructing:
                //instruct the player about objective

                if(!_audio.IsPlaying(AudioSourceType.Voice)) {
                    //instructions are done, so either start the objective, reinforce the objective or follow

                    //activate the current task and go back to the following state
                    _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Active);
                    _tracker.StartTracking(_controls.GetTrashCount());
                    SetState(CompanionState.Following);
                }

                break;

            case CompanionState.Transforming:
                //tranform companion into gun or back to robot

                switch(_transformationState) {
                    case TransformationState.Vacuum:
                        TransformToVacuum();
                        break;

                    case TransformationState.Robot:
                        TransformToRobot();
                        break;

                    default:
                        _navigator.SetAgentStatus(true);
                        CheckQueueState(); //either go to vacuum gun or follow state
                        break;
                }

                break;

            case CompanionState.Useable:
                //ready to be used as vacuum gun

                if(_controls.GrabButtonPressed() && _controls.InCollider()) {
                    SetState(CompanionState.Grabbed);
                } else if (_controls.CallButtonDown()) {
                    QueueState(CompanionState.Following);
                    SetState(CompanionState.Transforming);
                }

                break;

            case CompanionState.Grabbed:
                //currently used a vacuum gun

                if (!_controls.GrabButtonPressed()) {
                    //transform back
                    ClearQueue();
                    QueueState(CompanionState.Following);
                    SetState(CompanionState.Transforming);
                }

                //use vacuum gun
                if (_controls.UseButtonPressed()) {
                    _controls.UseVacuum(); //using Felix' vaccum gun script
                }

                break;

            default:
                break;
        }
    }

}