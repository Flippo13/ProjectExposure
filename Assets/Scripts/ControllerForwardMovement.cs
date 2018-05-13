using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ControllerForwardMovement : MonoBehaviour {

    public float speed;

    private List<XRNodeState> _trackedNodes;
    private string[] _joysticks;

    private XRNode _leftHandNode;
    private XRNode _rightHandNode;

    private Rigidbody _rigidbody;

    public void Awake() {
        XRSettings.enabled = true; //enable XR globally, probably not needed
        XRSettings.showDeviceView = true; //try to mirror what the vr device is seeing to the main screen
        //XRSettings.LoadDeviceByName("Oculus"); //force load the oculus, might not work
        if(XRSettings.isDeviceActive) Debug.Log("VR Device loaded: " + XRSettings.loadedDeviceName); //debug info about the device

        _trackedNodes = new List<XRNodeState>();

        InputTracking.GetNodeStates(_trackedNodes); //populates the list with all present nodes
        _joysticks = Input.GetJoystickNames();

        //store the needed nodes for the hands
        for(int i = 0; i < _trackedNodes.Count; i++) {
            Debug.Log("Detected Node: " + InputTracking.GetNodeName(_trackedNodes[i].uniqueID)); //print out available node types

            if (_trackedNodes[i].nodeType == XRNode.LeftHand) _leftHandNode = _trackedNodes[i].nodeType;
            if (_trackedNodes[i].nodeType == XRNode.RightHand) _rightHandNode = _trackedNodes[i].nodeType;
        }

        for(int i = 0; i < _joysticks.Length; i++) {
            Debug.Log("Detected Controller: " + _joysticks[i]); //print out available controllers
        }

        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Update() {
        bool move = false;

        if(Input.GetButton("Button 14") || Input.GetButton("Button 15")) {
            //primary index button?
            Debug.Log("Left hand position: " + InputTracking.GetLocalPosition(_leftHandNode));
            Debug.Log("Left hand rotation: " + InputTracking.GetLocalRotation(_leftHandNode));

            //secondary index button?
            Debug.Log("Right hand position: " + InputTracking.GetLocalPosition(_rightHandNode));
            Debug.Log("Right hand rotation: " + InputTracking.GetLocalRotation(_rightHandNode));

            move = true;
        }

        if(move) {
            Vector3 directionVec = InputTracking.GetLocalPosition(_rightHandNode) - InputTracking.GetLocalPosition(_leftHandNode); //vector pointing from the left to the right hand
            Vector3 movementDir = Vector3.Cross(Vector3.up, directionVec); //cross to determine the movement direction

            _rigidbody.AddForce(movementDir.normalized * speed); //move to the calculated direction
        }
    }

}