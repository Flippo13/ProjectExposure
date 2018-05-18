using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//state machine of the robot containing its behaviour
public class CompanionAI : MonoBehaviour {

    private CompanionState _currentState;

    public void Awake() {
        InitiateCompanion();
    }

    public void Update() {
        UpdateState();
    }

    public void SetState(CompanionState newState) {
        ExitState(_currentState);
        EnterState(newState);
    }

    public CompanionState GetCurrentState() {
        return _currentState;
    }

    private void InitiateCompanion() {
        _currentState = CompanionState.Inactive; //default state
    }

    private void EnterState(CompanionState state) {
        switch (state) {
            case CompanionState.Inactive:
                break;

            case CompanionState.Following:
                break;

            case CompanionState.Traveling:
                break;

            case CompanionState.Roaming:
                break;

            case CompanionState.Waiting:
                break;

            case CompanionState.Instructing:
                break;

            case CompanionState.Transforming:
                break;

            case CompanionState.Useable:
                break;

            default:
                break;
        }

        _currentState = state;
    }

    private void ExitState(CompanionState state) {
        switch (state) {
            case CompanionState.Inactive:
                break;

            case CompanionState.Following:
                break;

            case CompanionState.Traveling:
                break;

            case CompanionState.Roaming:
                break;

            case CompanionState.Waiting:
                break;

            case CompanionState.Instructing:
                break;

            case CompanionState.Transforming:
                break;

            case CompanionState.Useable:
                break;

            default:
                break;
        }
    }

    private void UpdateState() {
        switch(_currentState) {
            case CompanionState.Inactive:
                break;

            case CompanionState.Following:
                break;

            case CompanionState.Traveling:
                break;

            case CompanionState.Roaming:
                break;

            case CompanionState.Waiting:
                break;

            case CompanionState.Instructing:
                break;

            case CompanionState.Transforming:
                break;

            case CompanionState.Useable:
                break;

            default:
                break;
        }
    }

}
