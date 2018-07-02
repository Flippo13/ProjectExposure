using UnityEngine;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    public CompanionDestination companionDestination;
    public VacuumScript vacuum;
    public VacuumGrabScanner grabScanner;

    public float interactionRadius;
    public float playerSeperationRadius;

    public float idleAnimInterval;
    public float followTimeout;
    public float handOverTimeout;
    public float emergencyTimeout;

    public bool debug;
    public bool skipTutorial;

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
    private bool _instructedCall;

    public void Start() {
        //get all relevant components
        _controls = GetComponent<CompanionControls>();
        _navigator = GetComponent<CompanionNavigator>();
        _audio = GetComponent<CompanionAudio>();
        _animation = GetComponent<CompanionAnimation>();
        _tracker = GetComponent<CompanionObjectiveTracker>();
        _debug = GetComponent<CompanionDebug>();

        _wasCalled = false;
        _instructedCall = false;

        _debug.Init();
        _debug.SetRendererStatus(debug);

        if (skipTutorial) {
            //skip tutorial in debug
            EnterState(CompanionState.Following);
        } else {
            //set up tutorial
            _inTutorial = true;
            EnterState(CompanionState.Tutorial);
            InitTutorial();
        }
    }

    public void Update() {
        UpdateTracker();
        UpdateState();
    }

    public void CheckForCallInstruction(TutorialArea tutorialArea, TutorialWaypoint tutorialWaypoint) {
        if (_instructedCall) return;

        //instruct when vacuum can be picked up
        if (CheckForVacuumGrab() && _audio.GetPlaybackState(AudioSourceType.Voice) == FMOD.Studio.PLAYBACK_STATE.STOPPED && !_audio.GetStartedPlaying()) {
            //show tutorial and play voiceline
            tutorialWaypoint.Activate(tutorialArea);
            if(debug) Debug.Log("Activating call instruction");

            _instructedCall = true;
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

    private bool InInteractionRange() {
        float distance = Vector3.Distance(companionDestination.GetPosition(), transform.position);

        return distance <= interactionRadius; //returns true, if the companion is in the interaction range of the player
    }

    private bool InSeperationRange() {
        float distance = Vector3.Distance(companionDestination.GetPosition(), transform.position);

        return distance <= playerSeperationRadius; //returns true, if the companion is in the max seperation range of the player
    }

    private bool CheckForIdleAnimation() {
        return _idleTimer >= idleAnimInterval && !_animation.IsPlayingIdle();
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
        Vector3 deltaVec = companionDestination.GetPosition() - vacuum.transform.position;

        return deltaVec.magnitude > interactionRadius && vacuum.GetVacuumState() == VacuumState.Free; //returns true, if the player is not close to the vacuum and the vacuum is lying around
    }

    private bool CheckForCompanionCall() {
        if (_controls.CallButtonDown()) {
            //call the companion
            if (CheckForVacuumGrab()) {
                //get the vacuum if the vacuum is lying around
                _wasCalled = true;

                //audio and vibration feedback
                _audio.PlayCallFeedback();

                SetState(CompanionState.GettingVacuum);
                return true;
            } else if (vacuum.GetVacuumState() == VacuumState.CompanionBack || vacuum.GetVacuumState() == VacuumState.CompanionHand) {
                //otherwise only get called when the vacuum is on the companion

                //audio and vibration feedback
                _audio.PlayCallFeedback();

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

        if (mainObjective != null) { //only main objectives remaining
            _tracker.SetCurrentObjective(mainObjective);

            return true;
        }

        return false;
    }

    private void RotateTowardsPlayer() {
        //rotate the companion towards the player over the y axis (not smooth but snap)

        Vector3 targetPos = new Vector3(companionDestination.GetPosition().x, transform.position.y, companionDestination.GetPosition().z); //only rotate over y
        transform.LookAt(targetPos);
    }

    private void EnterState(CompanionState state) {
        if (debug) Debug.Log("Entering state " + state);

        switch (state) {

            case CompanionState.Following:
                _navigator.SetAgentStatus(true);
                _timer = 0f;

                break;

            case CompanionState.Returning:
                _navigator.SetAgentStatus(true);
                _timer = 0f;

                break;

            case CompanionState.Traveling:
                _navigator.SetAgentStatus(true);
                _navigator.SetDestination(_tracker.GetCurrentObjective().transform.position);

                break;

            case CompanionState.Waiting:
                _navigator.SetAgentStatus(false);
                _timer = float.MaxValue; //ensure to play the reinforcement once at the start
                _idleTimer = 0f;

                _tracker.GetCurrentObjective().SetPointerStatus(true); //enable the objective pointer

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
                _timer = 0f;

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

            case CompanionState.Waiting:
                _tracker.GetCurrentObjective().SetPointerStatus(false); //disable the objective pointer

                break;

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

                CheckForObjectives();
                _navigator.CheckForSpeedAdjustment(companionDestination.GetPosition()); //adjust speed based on the distance between player and companion

                if (CheckForCompanionCall()) return;

                //check wether the vacuum was dropped or not
                if (CheckForVacuumGrab()) {
                    _wasCalled = false;
                    SetState(CompanionState.GettingVacuum);
                    return;
                }

                if(InSeperationRange()) {
                    _timer += Time.deltaTime;

                    //if the idle timeout was reached or the player is in range of the objective
                    if (_timer >= followTimeout || _navigator.InRange(_tracker.GetCurrentObjective().transform.position, companionDestination.GetPosition(), playerSeperationRadius)) {
                        SetState(CompanionState.Traveling);  //try going to the objective when player is idle for too long
                        return;
                    }
                }

                if (!InInteractionRange()) { //if there is no objective in range and the player is out of range
                    //move to the player
                    Vector3 deltaVecPlayer = transform.position - companionDestination.GetPosition();
                    Vector3 destination = companionDestination.GetPosition() + deltaVecPlayer.normalized * interactionRadius; //player pos plus an offset

                    _navigator.SetDestination(destination);
                    _idleTimer = 0f; //reset timer
                } else {
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
                _navigator.CheckForSpeedAdjustment(companionDestination.GetPosition());

                if (!InInteractionRange()) {
                    //move to the player without other priorities
                    Vector3 deltaVecPlayer = transform.position - companionDestination.transform.position;
                    Vector3 destination = companionDestination.transform.position + deltaVecPlayer.normalized * interactionRadius;

                    if (debug) destination = companionDestination.GetDestinationPosition(interactionRadius);//experimental companion walking in front of the player

                    _navigator.SetDestination(destination);
                } else {
                    //close enough to the player
                    SetState(CompanionState.HandingVacuum);
                }

                _timer += Time.deltaTime;

                break;

            case CompanionState.Traveling:
                //travel to main objective
                _navigator.CheckForSpeedAdjustment(companionDestination.GetPosition());

                if (CheckForCompanionCall()) return;

                if(!InSeperationRange() || _navigator.InRange(_tracker.GetCurrentObjective().transform.position, 0.8f)) {
                    //player is not in the seperation range anymore
                    SetState(CompanionState.Waiting);
                }

                break;

            case CompanionState.Waiting:
                //reinforce the player to come to the objective
                RotateTowardsPlayer();

                if (CheckForCompanionCall()) return;

                if (InInteractionRange()) {
                    //if the player is close enough, start instructing
                    SetState(CompanionState.Instructing);
                    return;
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

                if(!_navigator.InRange(companionDestination.GetPosition(), playerSeperationRadius * 1.2f)) {
                    //player is not in range of the seperation randius anymore
                    SetState(CompanionState.Following);
                    return;
                }

                if (_navigator.InRange(companionDestination.GetPosition(), playerSeperationRadius * 0.7f) && !_navigator.InRange(_tracker.GetCurrentObjective().transform.position, 0.8f)) {
                    //player comes closer, so travel to objective
                    SetState(CompanionState.Traveling);
                    return;
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
                _navigator.CheckForSpeedAdjustment(companionDestination.GetPosition());

                if (_timer >= emergencyTimeout) {
                    //companion couldnt get the vacuum for a long time, so teleport it to the companion
                    vacuum.transform.position = transform.position;
                }

                Vector3 vacuumPos = vacuum.transform.position;

                _navigator.SetDestination(vacuumPos); //change in case it has altered             

                if (_navigator.InRange(vacuumPos, 1f)) {
                    vacuum.SetVacuumState(VacuumState.CompanionBack);

                    //return if he was called, following if not
                    if (_wasCalled) {
                        SetState(CompanionState.Returning);
                    } else {
                        SetState(CompanionState.Following);
                    }
                } else if (_navigator.InRange(vacuumPos, 4f)) {
                    _animation.SetGrabbingVaccumTrigger(); //play animation
                }

                _timer += Time.deltaTime;

                break;

            case CompanionState.HandingVacuum:
                //check if the vacuum is grabbed or if the player didnt grab it (in animation)S
                RotateTowardsPlayer();

                if (_animation.VacuumHandDone()) {
                    SetState(CompanionState.Following); //go back to overall idle
                    return;
                }

                if (grabScanner.IsReachingForVacuum()) {
                    //reset the timer when player is reaching out
                    _timer = 0f;
                } else if (_timer >= handOverTimeout && vacuum.GetVacuumState() != VacuumState.Player && vacuum.GetVacuumState() != VacuumState.Free) {
                    //put vacuum back
                    _animation.SetAnimationTrigger("hand_over_vacuum_back");
                } else if (vacuum.GetVacuumState() == VacuumState.Player || vacuum.GetVacuumState() == VacuumState.Free) {
                    //go back to hover idle when vacuum is grabbed or released
                    _animation.SetAnimationTrigger("hand_over_vacuum_hover");
                }

                _timer = _timer + Time.deltaTime;

                break;

            default:
                break;
        }
    }

}