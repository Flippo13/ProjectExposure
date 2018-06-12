using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

//objective information container
public class CompanionObjective : MonoBehaviour {

    //default information
    public ObjectiveType objectiveType;
    public ObjectiveTask objectiveTask;
    public string animationTrigger;

    [EventRef]
    public string instructionClip;

    [EventRef]
    public string reinforcementClip;

    public float reinforcementInterval;

    //task dependant information
    public int trashAmount;
    public SceneTransition sceneTransition;
    public CheckForTrashArea[] dropdownZones = new CheckForTrashArea[3];
    public Transform turbine;
    public PowerGrid powerGrid;
    public TurbineButtonActivate turbineButton;

    //states
    private ObjectiveStatus _status;
    private bool _chosePosition;
    private bool _allowPlugIn;
    private bool _allowButtonPress;

    public void Awake() {
        _status = ObjectiveStatus.Incomplete;

        InitObjective();
    }

    private void InitObjective() {
        switch(objectiveTask) {
            case ObjectiveTask.Choose:
                SetDropdownStatus(false); //disable choosing at first
                _chosePosition = false;

                break;

            case ObjectiveTask.Place:
                //needs no instruction and reinforcement clips, since the turbine dropdown is triggered immediatly
                instructionClip = "";
                reinforcementClip = "";

                break;

            case ObjectiveTask.PlugIn:
                //dont allow plugin detection
                _allowPlugIn = false;

                break;

            case ObjectiveTask.PowerOn:
                //dont allow button press detection
                _allowButtonPress = false;

                break;

            default:
                break;

        }
    }

    //set the dropdown zones active or inactive
    private void SetDropdownStatus(bool status) {
        for(int i = 0; i < dropdownZones.Length; i++) {

            if(!status) {
                dropdownZones[i].areaChosen.AddListener(PositionChosen); //adding my function as listener to check for activation
            }

            dropdownZones[i].enabled = status;
        }
    }

    public void SetStatus(ObjectiveStatus status) {
        _status = status;

        if(_status == ObjectiveStatus.Active) { //activation events
            switch(objectiveTask) {
                case ObjectiveTask.Choose:
                    //activate zones
                    SetDropdownStatus(true);

                    break;

                case ObjectiveTask.PlugIn:
                    //allowing to check for connection status of the cable
                    _allowPlugIn = true;

                    break;

                case ObjectiveTask.PowerOn:
                    //dont allow button press detection
                    _allowButtonPress = true;

                    break;

                default:
                    break;
            }

        } else if(_status == ObjectiveStatus.Complete) { //completion events

            switch (objectiveTask) {
                default:
                    break;
            }
        }
    }

    void PositionChosen() {
        _chosePosition = true;
    }

    public bool IsCompleted() {
        return _status == ObjectiveStatus.Complete;
    }

    public bool IsActive() {
        return _status == ObjectiveStatus.Active;
    }

    public bool ChosePosition() {
        return _chosePosition;
    }

    public bool DroppedTurbine() {
        //if the turbine is dropped it is unparented
        return turbine.parent == null;
    }

    public bool CableConnected() {
        return _allowPlugIn && powerGrid.Connected;
    }

    public bool ConsoleButtonPressed() {
        return _allowButtonPress && turbineButton.Active;
    }
}
