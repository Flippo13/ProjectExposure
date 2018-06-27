using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool _inTutorial;

    public void Start() {
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

        _inTutorial = true;
        EnterState(CompanionState.Tutorial);
        InitTutorial();
    }

    public void FixedUpdate() {
        UpdateTracker();
        UpdateState();
    }

    public void TutorialCall() {
        //call the companion
        if (CheckForVacuumGrab()) {
            //get the vacuum if the vacuum is lying around
            _wasCalled = true;
            SetState(CompanionState.GettingVacuum);
        } else if (vacuum.GetVacuumState() == VacuumState.CompanionBack || vacuum.GetVacuumState() == VacuumState.CompanionHand) {
            SetState(CompanionState.Returning);
        }
    }

    private void SetState(CompanionState newState) {
        ExitState(_aiState);
        EnterState(newState);
    }

    private void InitTutorial() {
        //look for tutorial objective and activate it
        _tracker.SetCurrentObjective(_tracker.GetTutorialObjective());
        if (_tracker.GetCurrentObjective() != null) _tracker.GetCurrentObjective().SetStatus(ObjectiveStatus.Active);

        vacuum.SetVacuumState(VacuumState.Free); //unparent vacuum
        vacuum.transform.position = _tracker.GetCurrentObjective().transform.position; //relocate vacuum gun to the tutorial objective location
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
        if (grabScanner.IsReachingForVacuum() && vacuum.GetVacuumState() != VacuumState.Player && vacuum.GetVacuumState() != VacuumState.Free) {
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
        if (_controls.CallButtonDown()) {
            //call the companion
            if (CheckForVacuumGrab()) {
                //get the vacuum if the vacuum is lying around
                _wasCalled = true;
                SetState(CompanionState.GettingVacuum);
                return true;
            } else if (vacuum.GetVacuumState() == VacuumState.CompanionBack || vacuum.GetVacuumState() == VacuumState.CompanionHand) {
                //otherwise only get called when the vacuum 
                SetState(CompanionState.Returning);
                return true;
            } else {
                return false;
            }
        }

        return false;
    }

    //returns true, if an objective was found
    private bool CheckForObjectives() {
        if (_tracker.GetCurrentObjective() != null && _tracker.GetCurrentObjective().IsActive()) return false; //an objective is already active

        //looking for an entirely new objective
        CompanionObjective mainObjective = _tracker.GetNextObjectiveInBranch();
        CompanionObjective sideObjective = _tracker.GetClosestSideObjective();

        if (mainObjective != null && sideObjective != null) {   //main and side remaining
            float mainDistance = _tracker.GetObjectiveDistance(mainObjective);
            float sideDistance = _tracker.GetObjectiveDistance(sideObjective);

            //prioritize main objective over side objective when both are in range
            if (mainDistance < objectiveScanRadius) {
                //if it is closer than the side objective and in scan radius
                _tracker.SetCurrentObjective(mainObjective);
                SetState(CompanionState.Traveling);

                return true;
            } else if (sideDistance < objectiveScanRadius) {
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
            if (debug) Debug.Log("No Main or Side Objectives found");
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

            case CompanionState.Following:
                _navigator.SetAgentStatus(true);

                break;

            case CompanionState.Returning:
                _navigator.SetAgentStatus(true);

                break;

            case CompanionState.Traveling:
                _navigator.SetAgentStatus(true);
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Roaming:
                _navigator.SetAgentStatus(true);
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Waiting:
                _navigator.SetAgentStatus(false);
                _timer = float.MaxValue; //ensure to play the reinforcement once at the start
                _idleTimer = 0f;

                break;

            case CompanionState.Instructing:
                _navigator.SetAgentStatus(false);
                _audio.StopAudioSource(AudioSourceType.Voice);

                if (_audio.SetClip(_tracker.GetCurrentObjective().instructionClip, AudioSourceType.Voice)) {
                    StartCoroutine(_audio.PlayAudioSourceWithHaptic(AudioSourceType.Voice));
                    _animation.SetAnimationTrigger(_tracker.GetCurrentObjective().instructionAnimationTrigger);
                }

                break;

            case CompanionState.GettingVacuum:
                _navigator.SetAgentStatus(true);
                _animation.SetPlayingGrab(false); //reset grab animation

                break;

            case CompanionState.HandingVacuum:
                _timer = 0f;
                _navigator.SetAgentStatus(false);
                _animation.SetAnimationTrigger("hand_over_vacuum_hand"); //start handing over animation

                break;

            default:
                break;
        }

        _aiState = state;
        if (debug) _debug.ApplyState(_aiState);
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
            case CompanionState.Tutorial:
                //minimalistic following state for the tutorial
                RotateTowardsPlayer();

                if (_tracker.GetCurrentObjective().tutorialArea.GetCurrentTutorialButton() == TutorialButtons.None) return;

                if (_tracker.GetCurrentObjective().IsCompleted()) {
                    _inTutorial = false;
                    SetState(CompanionState.Following); //tutorial is completed, so return to normal behaviour
                }

                break;

            case CompanionState.Following:
                //idle/main state of the companion

                if (_inTutorial) { //go back to tutorial instead of doing the other stuff
                    SetState(CompanionState.Tutorial);
                    return;
                }

                _navigator.CheckForSpeedAdjustment(companionDestination.position); //adjust speed based on the distance between player and companion

                if (CheckForCompanionCall()) return;

                //check wether the vacuum was dropped or not
                if (CheckForVacuumGrab()) {
                    _wasCalled = false;
                    SetState(CompanionState.GettingVacuum);
                    return;
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
                } else {
                    //close enough to the player
                    SetState(CompanionState.HandingVacuum);
                }

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

                    if (_audio.SetClip(_tracker.GetCurrentObjective().reinforcementClip, AudioSourceType.Voice)) {
                        StartCoroutine(_audio.PlayAudioSourceWithHaptic(AudioSourceType.Voice));
                        _animation.SetAnimationTrigger(_tracker.GetCurrentObjective().reinforcementAnimationTrigger);
                    }
                } else if (CheckForIdleAnimation()) {
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
                    if (_wasCalled) {
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
                    return;
                }

                if (vacuum.GetVacuumState() == VacuumState.Player || vacuum.GetVacuumState() == VacuumState.Free) {
                    //go back to hover idle when vacuum is grabbed or released
                    _animation.SetAnimationTrigger("hand_over_vacuum_hover");
                    return;
                }

                if (grabScanner.IsReachingForVacuum()) {
                    //reset the timer when player is reaching out
                    _timer = 0f;
                } else if (_timer >= 1.5f && vacuum.GetVacuumState() != VacuumState.Player && vacuum.GetVacuumState() != VacuumState.Free) {
                    //put vacuum back
                    _animation.SetAnimationTrigger("hand_over_vacuum_back");
                    return;
                }

                _timer = _timer + Time.deltaTime;

                break;

            default:
                break;
        }
    }

}