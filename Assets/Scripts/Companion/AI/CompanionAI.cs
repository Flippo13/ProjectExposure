using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    public Transform companionDestination;
    public VacuumScript vacuum;
    public VacuumGrabScanner grabScanner;

    public float interactionRadius;
    public float objectiveScanRadius;

    public float stayDuration;
    public float idleInterval;

    public bool debug;

    private CompanionState _aiState;

    private CompanionControls _controls;
    private CompanionNavigator _navigator;
    private CompanionAudio _audio;
    private CompanionAnimation _animation;
    private CompanionObjectiveTracker _tracker;
    private CompanionDebug _debug;

    private float _timer;
    private float _idleTimer;
    private bool _wasCalled;

    public void Awake() {
        //get all relevant components
        _controls = GetComponent<CompanionControls>();
        _navigator = GetComponent<CompanionNavigator>();
        _audio = GetComponent<CompanionAudio>();
        _animation = GetComponent<CompanionAnimation>();
        _tracker = GetComponent<CompanionObjectiveTracker>();
        _debug = GetComponent<CompanionDebug>();

        _wasCalled = false;

        _debug.Init();
        _debug.SetRendererStatus(debug);

        if (SceneManager.GetActiveScene().buildIndex == 0) EnterState(CompanionState.Following); //level 1
        else EnterState(CompanionState.Following);
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
        Vector3 deltaVec = companionDestination.position - transform.position;

        return deltaVec.magnitude <= interactionRadius; //returns true, if the companion is in the interaction range of the player
    }

    private bool CheckForIdleAnimation() {
        return _idleTimer >= idleInterval && !_animation.IsPlayingIdle();
    }

    private bool CheckForVacuumHandOver() {
        //go into new state if player is reaching out for vacuum
        if(grabScanner.IsReachingForVacuum() && vacuum.GetVacuumState() != VacuumState.Player && vacuum.GetVacuumState() != VacuumState.Free) {
            SetState(CompanionState.HandingVacuum);

            return true;
        }

        return false;
    }

    private bool CheckForVacuumGrab() {
        Vector3 deltaVec = companionDestination.position - vacuum.transform.position;

        return deltaVec.magnitude > interactionRadius && vacuum.GetVacuumState() == VacuumState.Free; //returns true, if the player is not close to the vacuum and the vacuum is lying around
    }

    private bool CheckForCompanionCall() {
        if (_controls.CallButtonDown() || Input.GetKeyDown(KeyCode.Q)) {
            //call the companion
            if (CheckForVacuumGrab()) {
                //get the vacuum if the vacuum is lying around
                _wasCalled = true;
                SetState(CompanionState.GettingVacuum);
            } else {
                SetState(CompanionState.Returning);
            }
            
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
            if(debug) Debug.Log("No Main or Side Objectives found");
        }

        //nothing in scan radius
        return false;
    }

    private void RotateTowardsPlayer() {
        //rotate the companion towards the player over the y axis (not smooth but snap)

        Vector3 targetPos = new Vector3(companionDestination.position.x, transform.position.y, companionDestination.position.z); //only rotate over y
        transform.LookAt(targetPos);
    }

    private void EnterState(CompanionState state) {
        if (debug) Debug.Log("Entering state " + state);

        switch (state) {

            case CompanionState.Staying:
                _timer = 0f;

                break;

            case CompanionState.Traveling:
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Roaming:
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Waiting:
                _timer = float.MaxValue; //ensure to play the reinforcement once at the start
                _idleTimer = 0f;

                break;

            case CompanionState.Instructing:
                _audio.StopAudioSource(AudioSourceType.Voice);
                if (_audio.SetClip(_tracker.GetCurrentObjective().instructionClip, AudioSourceType.Voice)) StartCoroutine(_audio.PlayAudioSourceWithHaptic(AudioSourceType.Voice));
                _animation.SetAnimationTrigger(_tracker.GetCurrentObjective().animationTrigger);

                break;

            case CompanionState.GettingVacuum:
                _animation.SetPlayingGrab(false); //reset grab animation

                break;

            case CompanionState.HandingVacuum:
                _animation.SetAnimationTrigger("hand_over_vacuum_hand"); //start handing over animation

                break;

            default:
                break;
        }

        _aiState = state;
        if(debug) _debug.ApplyState(_aiState);
    }

    private void ExitState(CompanionState state) {
        if (debug) Debug.Log("Leaving state " + state);

        switch (state) {
            default:
                break;
        }
    }

    private void UpdateTracker() {
        if (_tracker.GetCurrentObjective() != null && _tracker.GetCurrentObjective().IsActive()) {
            //track progress
            if (!_tracker.TrackObjective(_controls.GetTrashCount())) {
                //if the objective is completed
                _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Complete);
                if (debug) Debug.Log("Objective complete");
            }
        }
    }

    private void UpdateState() {
        switch (_aiState) {
            case CompanionState.Following:
                //idle/main state of the companion

                _navigator.CheckForSpeedAdjustment(companionDestination.position); //adjust speed based on the distance between player and companion

                if (CheckForCompanionCall()) return;

                //check wether the vacuum was dropped or not
                if (CheckForVacuumGrab()) {
                    _wasCalled = false;
                    SetState(CompanionState.GettingVacuum);
                }

                if (!CheckForObjectives() && !InInterationRange()) { //if there is no objective in range and the player is out of range
                    //move to the player
                    Vector3 deltaVecPlayer = transform.position - companionDestination.transform.position;
                    Vector3 destination = companionDestination.transform.position + deltaVecPlayer.normalized * interactionRadius; //player pos plus an offset

                    _navigator.SetDestination(destination);
                    _idleTimer = 0f; //reset timer
                } else if (InInterationRange()) {
                    //next to the player
                    RotateTowardsPlayer();
                    if (CheckForVacuumHandOver()) return;

                    if (CheckForIdleAnimation()) {
                        _animation.SetRandomIdle(); //play idle animation
                        _idleTimer = 0f;
                    }

                    _idleTimer += Time.deltaTime;
                }

                break;

            case CompanionState.Returning:
                //returning to the player when called
                _navigator.CheckForSpeedAdjustment(companionDestination.position);

                if (!InInterationRange()) {
                    //move to the player without other priorities
                    Vector3 deltaVecPlayer = transform.position - companionDestination.transform.position;
                    Vector3 destination = companionDestination.transform.position + deltaVecPlayer.normalized * interactionRadius;

                    _navigator.SetDestination(destination);
                }

                //close enough to the player
                if (_navigator.ReachedDestinaton()) SetState(CompanionState.Staying);

                break;

            case CompanionState.Staying:
                //staying at its position after getting called
                RotateTowardsPlayer();
                if (CheckForVacuumHandOver()) return;

                if (_timer >= stayDuration) SetState(CompanionState.Following);

                _timer += Time.deltaTime;

                break;

            case CompanionState.Traveling:
                //travel to main objective
                _navigator.CheckForSpeedAdjustment(companionDestination.position);

                if (CheckForCompanionCall()) return;

                if (_navigator.ReachedDestinaton()) SetState(CompanionState.Waiting);

                break;

            case CompanionState.Roaming:
                //roam to side objective
                _navigator.CheckForSpeedAdjustment(companionDestination.position);

                if (CheckForCompanionCall()) return;

                if (_navigator.ReachedDestinaton()) SetState(CompanionState.Waiting);

                break;

            case CompanionState.Waiting:
                //reinforce the player to come to the objective
                RotateTowardsPlayer();

                if (CheckForCompanionCall()) return;

                if (InInterationRange()) {
                    //if the player is close enough, start instructing
                    SetState(CompanionState.Instructing);
                } else if (_timer >= _tracker.GetCurrentObjective().reinforcementInterval) {
                    //wait and reinforce the player for objective
                    _timer = 0f;
                    _audio.StopAudioSource(AudioSourceType.Voice);
                    if(_audio.SetClip(_tracker.GetCurrentObjective().reinforcementClip, AudioSourceType.Voice)) StartCoroutine(_audio.PlayAudioSourceWithHaptic(AudioSourceType.Voice));
                }

                if (CheckForIdleAnimation()) {
                    _animation.SetRandomIdle(); //play idle animation
                    _idleTimer = 0f;
                }

                _idleTimer += Time.deltaTime;
                _timer += Time.deltaTime;

                break;

            case CompanionState.Instructing:
                //instruct the player about objective
                RotateTowardsPlayer();

                if (_audio.GetPlaybackState(AudioSourceType.Voice) == FMOD.Studio.PLAYBACK_STATE.STOPPED && _audio.GetStartedPlaying()) {
                    //instructions are done, so either start the objective, reinforce the objective or follow

                    //activate the current task and go back to the following state
                    _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Active);
                    _tracker.StartTracking(_controls.GetTrashCount());
                    _audio.ResetStartedPlaying();
                    SetState(CompanionState.Following);
                }

                break;

            case CompanionState.GettingVacuum:
                //pick up or catch the vacuum gun
                _navigator.CheckForSpeedAdjustment(companionDestination.position);

                Vector3 vacuumPos = vacuum.transform.position;
                Vector3 deltaVecVacuum = vacuumPos - transform.position;

                _navigator.SetDestination(vacuumPos);

                if (deltaVecVacuum.magnitude <= 1f) {
                    vacuum.SetVacuumState(VacuumState.CompanionBack);
                    
                    //return if he was called, following if not
                    if(_wasCalled) {
                        SetState(CompanionState.Returning); //includes stay
                    } else {
                        SetState(CompanionState.Following);
                    }
                } else if (deltaVecVacuum.magnitude <= 4f) {
                    _animation.SetGrabbingVaccumTrigger(); //play animation
                }

                break;

            case CompanionState.HandingVacuum:
                //check if the vacuum is grabbed or if the player didnt grab it (in animation)S
                RotateTowardsPlayer();

                if (_animation.VacuumHandDone()) {
                    SetState(CompanionState.Following); //go back to overall idle
                } else if (vacuum.GetVacuumState() == VacuumState.Player || vacuum.GetVacuumState() == VacuumState.Free) {
                    //go back to hover idle when vacuum is grabbed or released
                    _animation.SetAnimationTrigger("hand_over_vacuum_hover");
                } else if(!grabScanner.IsReachingForVacuum()) {
                    //put vacuum back
                    _animation.SetAnimationTrigger("hand_over_vacuum_back");
                }

                break;

            default:
                break;
        }
    }

}