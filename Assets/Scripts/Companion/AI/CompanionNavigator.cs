using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionNavigator : MonoBehaviour {

    public float agentSpeed;
    public float agentAcceleration;

    private NavMeshAgent _navAgent;
    private Rigidbody _rigidbody;
    private OVRGrabbable _grabbable;

    private bool _onGround;

    public void Awake() {
        _navAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _grabbable = GetComponent<OVRGrabbable>();

        SetAgentStatus(true);
        ResetSpeedAndAcceleration(); //set to default
    }

    public void OnCollisionEnter(Collision collision) {
        if (_navAgent.enabled) return; //only execute when the companion touches something while the navmeshagent in disabled

        _onGround = true;
    }

    //set destination for the navmesh agent
    public void SetDestination(Vector3 destination) {
        _navAgent.SetDestination(destination);
    }

    //enable or disable the navmesh agent
    public void SetAgentStatus(bool status) {
        _navAgent.enabled = status;
        _rigidbody.isKinematic = status;
        _grabbable.enabled = !status;
    }

    public void SetGrabbableStatus(bool status) {
        _grabbable.enabled = status;
    }

    public void SetAcceleration(float acceleration) {
        _navAgent.acceleration = acceleration;
    }

    public void SetSpeed(float speed) {
        _navAgent.speed = speed;
    }

    public void ResetSpeedAndAcceleration() {
        _navAgent.speed = agentSpeed;
        _navAgent.acceleration = agentAcceleration;
    }

    public bool ReachedDestinaton() {
        return _navAgent.remainingDistance <= 0.1f;
    }

    public void ResetOnGround() {
        _onGround = false;
    }

    public bool OnGround() {
        return _onGround;
    }
}
