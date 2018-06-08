using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//helper component for keeping track of the objectives
public class CompanionObjectiveTracker : MonoBehaviour {

    public Transform objectiveLog;

    private List<CompanionObjective> _mainObjectives;
    private List<CompanionObjective> _sideObjectives;

    private CompanionObjective _currentObjective;

    private int _startTrash;

    public void Awake() {
        if (objectiveLog == null) Debug.LogWarning("WARNING: ObjectiveLog reference in CompanionObjectiveTracker is missing!");

        _mainObjectives = new List<CompanionObjective>();
        _sideObjectives = new List<CompanionObjective>();

        CompanionObjective[] allObjectives = objectiveLog.GetComponentsInChildren<CompanionObjective>();

        //sorting out objectives
        for(int i = 0; i < allObjectives.Length; i++) {
            if (allObjectives[i].objectiveType == ObjectiveType.Main) _mainObjectives.Add(allObjectives[i]);
            else if (allObjectives[i].objectiveType == ObjectiveType.Side) _sideObjectives.Add(allObjectives[i]);
        }

        _currentObjective = _mainObjectives[0];
    }

    public void SetCurrentObjective(CompanionObjective objective) {
        _currentObjective = objective;
    }

    public CompanionObjective GetCurrentObjective() {
        return _currentObjective;
    }

    //returns the next main objective that isnt completed, returns null if no objective was found
    public CompanionObjective GetNextMainObjective() {
        CompanionObjective mainObjective = null;

        //get the next uncompleted main objective
        for(int i = 0; i < _mainObjectives.Count; i++) {
            if(!_mainObjectives[i].IsCompleted()) {
                mainObjective = _mainObjectives[i];
                break;
            }
        }

        return mainObjective;
    }

    //returns the closest side objective in range that isnt completed, returns null if no objective was found
    public CompanionObjective GetClosestSideObjective() {
        CompanionObjective closestObjective = null;
        float prevMagnitude = 0f;

        //using sqrMagnitude for better performance, since we are only comparing them against each other
        for (int i = 0; i < _sideObjectives.Count; i++) {
            if (_sideObjectives[i].IsCompleted()) continue;

            float magnitude = (_sideObjectives[i].transform.position - transform.position).sqrMagnitude;

            if(closestObjective == null || magnitude < prevMagnitude) {
                closestObjective = _sideObjectives[i];
                prevMagnitude = magnitude;
            }
        }

        return closestObjective;
    }

    public float GetObjectiveDistance(CompanionObjective objective) {
        return (objective.transform.position - transform.position).magnitude;
    }

    public void StartTracking(int trashCount) {
        _startTrash = trashCount;
    }

    //returns true, if the objective is still being tracked, otherwise returns false
    public bool TrackObjective(int trashCount) {
        switch (_currentObjective.objectiveTask) {
            case ObjectiveTask.Talk:
                return false; //needs no tracking

            case ObjectiveTask.Cleanup:
                //collected enough trash
                if (trashCount - _startTrash >= _currentObjective.trashAmount) return false;

                break;

            case ObjectiveTask.Choose:
                break;

            case ObjectiveTask.Place:
                break;

            case ObjectiveTask.PlugIn:
                break;

            case ObjectiveTask.PowerOn:
                break;

            case ObjectiveTask.Assemble:

                break;

            case ObjectiveTask.NextLevel:
                _currentObjective.sceneTransition.SetStatus(true); //activate the transition collider
                return false;

            case ObjectiveTask.Event:
                break;

            default:
                break;
        }

        return true;
    }
}
