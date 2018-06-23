﻿using System.Collections;
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

    //returns the first objective is it is the tutorial one, otherwise returns null
    public CompanionObjective GetTutorialObjective() {
        if (_mainObjectives[0].objectiveTask == ObjectiveTask.Tutorial) return _mainObjectives[0];
        else return null;
    }

    //returns the closest main objective that isnt completed, returns null if no objective was found
    public CompanionObjective GetClostestMainObjective() {
        CompanionObjective closestObjective = null;
        float prevMagnitude = 0f;

        //using sqrMagnitude for better performance, since we are only comparing them against each other
        for (int i = 0; i < _mainObjectives.Count; i++) {
            if (_mainObjectives[i].IsCompleted()) continue;

            float magnitude = (_mainObjectives[i].transform.position - transform.position).sqrMagnitude;

            if (closestObjective == null || magnitude < prevMagnitude) {
                closestObjective = _mainObjectives[i];
                prevMagnitude = magnitude;
            }
        }

        return closestObjective;
    }

    //returns the closest side objective that isnt completed, returns null if no objective was found
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

            case ObjectiveTask.Tutorial:
                //tutorial completed
                if (_currentObjective.tutorialArea.TutorialCompleted()) {
                    _currentObjective.tutorialArea.SetBoundaryStatus(false); //disable boundaries
                    return false;
                }

                break;

            case ObjectiveTask.Cleanup:
                //collected enough trash
                if (trashCount - _startTrash >= _currentObjective.trashAmount) {
                    ScoreTracker.CompletedTurbines++;
                    return false;
                }

                break;

            case ObjectiveTask.Choose:
                //player chose position
                if (_currentObjective.ChosePosition()) return false;

                break;

            case ObjectiveTask.Place:
                //player dropped turbine
                if (_currentObjective.DroppedTurbine()) {
                    ScoreTracker.CompletedTurbines++;
                    return false;
                }

                break;

            case ObjectiveTask.PlugIn:
                //player has plugged in the cable socket into the power grid
                if (_currentObjective.CableConnected()) return false;

                break;

            case ObjectiveTask.PowerOn:
                //player has pressed the console button to reactivate the turbine
                if (_currentObjective.ConsoleButtonPressed()) return false;

                break;

            case ObjectiveTask.Assemble:
                if(_currentObjective.AssembledTurbine()) { //replace with real condition
                    ScoreTracker.CompletedTurbines++;
                    return false;
                }

                break;

            case ObjectiveTask.NextLevel:
                _currentObjective.sceneTransition.EnableCollider(); //activate the transition collider
                return false;

            case ObjectiveTask.Event:
                break;

            default:
                break;
        }

        return true;
    }
}
