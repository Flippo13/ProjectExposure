using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    public Transform companionDestination;

    public float interactionRadius;
    public float grabRadius;
    public float scanRadius;

    public float maxIdleTime;

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
    private CompanionGrabber _grabber;
    private CompanionDebug _debug;

    private float _timer;

    public void Awake() {
        _stateQueue = new List<CompanionState>();

        _controls = GetComponent<CompanionControls>();
        _navigator = GetComponent<CompanionNavigator>();
        _model = GetComponent<CompanionModel>();
        _audio = GetComponent<CompanionAudio>();
        _animation = GetComponent<CompanionAnimation>();
        _tracker = GetComponent<CompanionObjectiveTracker>();
        _grabber = companionDestination.GetComponent<CompanionGrabber>();
        _debug = GetComponent<CompanionDebug>();

        //if first boat scene: Inactive, otherwise: Following
        _debug.Init();
        _debug.SetRendererStatus(debug);
        EnterState(CompanionState.Inactive);
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
        Vector3 deltaVec = companionDestination.transform.position - transform.position;

        return deltaVec.magnitude <= interactionRadius; //returns true, if the companion is in the interaction range of the player
    }

    private bool CheckForCompanionCall() {
        if (_controls.CallButtonDown() || Input.GetKeyDown(KeyCode.Q)) {
            //call the companion and let it transform into the vacuum gun
            ClearQueue();
            QueueState(CompanionState.Useable);
            SetState(CompanionState.Transforming);

            return true;
        }

        return false;
    }

    private bool CheckForObjectives() {
        //maybe bugged?

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

        if(deltaVec.magnitude > grabRadius) {
            //travel to the hand (super fast)
            _navigator.SetSpeed(50f);
            _navigator.SetAcceleration(300f);
            _navigator.SetDestination(companionDestination.transform.position - deltaVec.normalized * grabRadius); //slight offset
        } else {
            //destination reached
            _navigator.ResetSpeedAndAcceleration();
            _transformationState = TransformationState.None;
        }
    }

    private void TransformToRobot() {
        //if the animation is done and the navigator is on the ground again
        if(_animation.TransformedBack() && _navigator.OnGround()) {
            //when done
            _transformationState = TransformationState.None;
        }
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
        //Debug.Log("Entering state " + state);

        switch (state) {

            case CompanionState.Following:
                _model.ActivateRobot();
                _navigator.SetAgentStatus(true);
                break;

            case CompanionState.Traveling:
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Roaming:
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Waiting:
                _timer = float.MaxValue;

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
                        _model.ActivateTransformation();
                        _animation.SetVacuumState(true);
                    } else if(_stateQueue[0] == CompanionState.Following) {
                        _transformationState = TransformationState.Robot;
                        _model.ActivateTransformation();
                        _animation.SetVacuumState(false);
                    }
                }

                break;

            case CompanionState.Useable:
                _timer = 0f;
                _navigator.SetAgentStatus(false);

                break;

            case CompanionState.Grabbed:
                _debug.SetRendererStatus(false);
                _model.ActivateVacuum();

                _navigator.SetGrabbableStatus(true);
                //_grabber.BeginGrabbing();

                break;

            default:
                break;
        }

        _aiState = state;
        _debug.ApplyState(_aiState);
    }

    private void ExitState(CompanionState state) {
        //Debug.Log("Leaving state " + state);

        switch (state) {
            case CompanionState.Grabbed:
                _debug.SetRendererStatus(debug);
                //_grabber.StopGrabbing(); //end grabbing
                _navigator.SetGrabbableStatus(false); //disable grabbale script
                _navigator.ResetOnGround(); //reset the on ground bool to check for ground collision

                transform.position = companionDestination.transform.position + companionDestination.forward.normalized;

                break;

            default:
                break;
        }
    }

    //maybe split this up into different functions or even classes if it becomes to much
    private void UpdateState() {

        switch(_aiState) {
            case CompanionState.Inactive:
                //activate the companion whn pressing the call button
                if (_controls.CallButtonDown() || Input.GetKeyDown(KeyCode.Q)) {
                    SetState(CompanionState.Following);
                }
                break;

            case CompanionState.Following:
                //idle/main state of the companion

                if (CheckForCompanionCall()) return; //prioritise calling

                if (!CheckForObjectives() && !InInterationRange()) { //if there is no objective in range and the player is out of range
                    //move to the player
                    Vector3 deltaVec = transform.position - companionDestination.transform.position;
                    Vector3 destination = companionDestination.transform.position + deltaVec.normalized * interactionRadius;

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
                } else if(_timer >= _tracker.GetCurrentObjective().reinforcementInterval) { //maybe cache the current objective
                    //wait and reinforce the player for objective
                    _timer = 0f;
                    _audio.SetClip(_tracker.GetCurrentObjective().reinforcementClip, AudioSourceType.Voice);
                    _audio.PlayAudioSource(AudioSourceType.Voice);
                }

                _timer += Time.deltaTime;

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
                        CheckQueueState(); //either go to vacuum gun or follow state
                        break;
                }

                break;

            case CompanionState.Useable:
                //ready to be used as vacuum gun

                if(_controls.GrabButtonPressed() && _grabber.InGrabCollider()) {
                    Debug.Log("Companion Grab pressed");
                    SetState(CompanionState.Grabbed);
                } else if (_controls.CallButtonDown() || Input.GetKeyDown(KeyCode.Q) || _timer >= maxIdleTime) {
                    //if the call button was pressed again or the companion remained idle for too long
                    ClearQueue();
                    QueueState(CompanionState.Following);
                    SetState(CompanionState.Transforming);
                }

                _timer += Time.deltaTime;

                break;

            case CompanionState.Grabbed:
                //currently used a vacuum gun

                if (!_controls.GrabButtonPressed()) {
                    Debug.Log("Companion Grab released");

                    //transform back
                    ClearQueue();
                    QueueState(CompanionState.Following);
                    SetState(CompanionState.Transforming);
                    return;
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