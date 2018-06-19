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

    public void Awake() {
        _waypoints = waypointHolder.GetComponentsInChildren<TutorialWaypoint>();
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
        if (_completed) return;

        _activeWaypoint.Deactivate();
        _activeWaypointIndex++;

        if(_activeWaypointIndex >= _waypoints.Length) {
            //completed all waypoints, so tutorial is done
            _completed = true;
            return;
        }

        //assign new active waypoint if possible
        _activeWaypoint = _waypoints[_activeWaypointIndex];
        _activeWaypoint.Activate(this);
    }

    public void SetBoundaryStatus(bool status) {
        boundaries.gameObject.SetActive(status);
    }

    public TutorialButtons GetCurrentTutorialButton() {
        if (_activeWaypoint == null) return TutorialButtons.None;
        else return _activeWaypoint.tutorialButton;
    }
}
