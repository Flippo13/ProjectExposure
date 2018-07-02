using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArea : MonoBehaviour {

    public ButtonsTutorial buttonsTutorial;
    public VacuumScript vacuum;
    public CompanionAudio companionAudio;
    public Transform waypointHolder;
    public Transform boundaries;

    private bool _completed;
    private TutorialWaypoint[] _waypoints;

    private int _activeWaypointIndex;
    private TutorialWaypoint _activeWaypoint;

    private CompanionAI _ai;
    private TutorialWaypoint _callWaypoint;

    public void Awake() {
        _waypoints = waypointHolder.GetComponentsInChildren<TutorialWaypoint>();
        _callWaypoint = _waypoints[_waypoints.Length - 1]; //last one should be the call

        _ai = companionAudio.GetComponent<CompanionAI>();
    }

    public void Update() {
        if (!_completed) return;

        //only check when tutorial is over
        _ai.CheckForCallInstruction(this, _callWaypoint);
    }

    public void StartTutorial() {
        _completed = false;

        _activeWaypointIndex = 0;
        _activeWaypoint = _waypoints[_activeWaypointIndex];
        _activeWaypoint.Activate(this);

        SetBoundaryStatus(true); //active boundaries
    }

    public bool TutorialCompleted() {
        return _completed;
    }

    public void ActivateNextWaypoint() {
        if(_activeWaypoint.IsActive()) _activeWaypoint.Deactivate();
        if (_completed) return;

        _activeWaypointIndex++;

        if(_activeWaypointIndex >= _waypoints.Length - 1) {
            //completed all waypoints until show buttons, so tutorial is done
            _completed = true;
            return;
        }

        //assign new active waypoint if possible
        _activeWaypoint.Activate(this);
    }

    public void Activate(TutorialWaypoint newActiveWaypoint) {
        _activeWaypoint = newActiveWaypoint;
    }

    public void SetBoundaryStatus(bool status) {
        boundaries.gameObject.SetActive(status);
    }

    public TutorialButtons GetCurrentTutorialButton() {
        if (_activeWaypoint == null) return TutorialButtons.None;
        else return _activeWaypoint.tutorialButton;
    }
}
