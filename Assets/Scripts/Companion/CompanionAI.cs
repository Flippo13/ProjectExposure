using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    public Transform player;
    public Transform companionAnchor;

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
    private CompanionObjectiveTracker _tracker;
    private CompanionDebug _debug;

    public void Awake() {
        _stateQueue = new List<CompanionState>();

        _controls = GetComponent<CompanionControls>();
        _navigator = GetComponent<CompanionNavigator>();
        _tracker = GetComponent<CompanionObjectiveTracker>();
        _debug = GetComponent<CompanionDebug>();

        //if first boat scene: Inactive, otherwise: Following
        EnterState(CompanionState.Following);
        _transformationState = TransformationState.None;
        _debug.SetRendererStatus(debug);
    }

    public void Update() {
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
            SetState(CompanionState.Transforming);
            QueueState(CompanionState.Useable);

            return true;
        }

        return false;
    }

    private bool CheckForObjectives() {
        CompanionObjective mainObjective = _tracker.GetNextMainObjective();
        CompanionObjective sideObjective = _tracker.GetClosestSideObjective();

        float mainDistance = _tracker.GetObjectiveDistance(mainObjective);
        float sideDistance = _tracker.GetObjectiveDistance(sideObjective);

        float closest = Mathf.Min(mainDistance, sideDistance);

        if(closest == mainDistance && mainDistance < scanRadius) {
            //if it is closer than the side objective and in scan radius
            _tracker.SetCurrentObjective(mainObjective);
            SetState(CompanionState.Traveling);

            return true;
        } else if(closest == sideDistance && sideDistance < scanRadius) {
            //if it is closer than the main objective and in scan radius
            _tracker.SetCurrentObjective(sideObjective);
            SetState(CompanionState.Roaming);

            return true;
        }

        //nothing in scan radius
        return false;
    }

    private void TransformToVacuum() {
        Vector3 deltaVec = companionAnchor.transform.position - transform.position;
        _navigator.SetAgentStatus(false);

        if(deltaVec.magnitude <= transformationRadius) {
            //play transformation animation
        }

        if(deltaVec.magnitude > 0) {
            //travel to the hand
            transform.Translate(deltaVec.normalized / 2f); 
        } else {
            //destination reached
            _transformationState = TransformationState.None;
        }
    }

    private void TransformToRobot() {
        _navigator.SetAgentStatus(false);

        //play transformation animation

        //when done
        _transformationState = TransformationState.None;
    }

    private void EnterState(CompanionState state) {
        Debug.Log("Entering state " + state);

        switch (state) {

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
                transform.parent = companionAnchor;
                _debug.SetRendererStatus(debug);
                _navigator.SetAgentStatus(false);

                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                
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
                transform.parent = null;
                _debug.SetRendererStatus(debug);
                _navigator.SetAgentStatus(true);

                transform.position = new Vector3(companionAnchor.position.x, 0.5f, companionAnchor.position.z);
                transform.rotation = Quaternion.identity;

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
                    SetState(CompanionState.Instructing);
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

                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Roaming:
                //roam to side objective
                if (CheckForCompanionCall()) return;

                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Waiting:
                //wait and reinforce the player for objective
                break;

            case CompanionState.Instructing:
                //instruct the player about objective
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
                }

                break;

            case CompanionState.Grabbed:
                //currently used a vacuum gun

                if (!_controls.GrabButtonPressed()) {
                    _navigator.Push(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch), 1f); //try to get the controller acceleration

                    //transform back
                    ClearQueue();
                    SetState(CompanionState.Transforming);
                    QueueState(CompanionState.Following);
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