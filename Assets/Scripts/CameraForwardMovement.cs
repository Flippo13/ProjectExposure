using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraForwardMovement : MonoBehaviour {

    public float speed;

    private string[] _joysticks;

    private Rigidbody _rigidbody;

    public void Awake() {
        XRSettings.enabled = true; //enable XR globally, probably not needed
        XRSettings.showDeviceView = true; //try to mirror what the vr device is seeing to the main screen
        //XRSettings.LoadDeviceByName("Oculus"); //force load the oculus, might not work
        if (XRSettings.isDeviceActive) Debug.Log("VR Device loaded: " + XRSettings.loadedDeviceName); //debug info about the device

        _joysticks = Input.GetJoystickNames();

        for (int i = 0; i < _joysticks.Length; i++) {
            Debug.Log("Detected Controller: " + _joysticks[i]); //print out available controllers
        }

        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Update() {
        if(Input.GetButton("Button 14") || Input.GetButton("Button 15")) {
            //pressing either of the index finger buttons?
            _rigidbody.AddForce(Camera.main.transform.forward.normalized * speed);
        }
    }
}
