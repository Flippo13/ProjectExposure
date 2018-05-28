using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionNavigator : MonoBehaviour {

    private NavMeshAgent _navAgent;
    private Rigidbody _rigidbody;

    public void Awake() {
        _navAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = true;
    }

    //set destination for the navmesh agent
    public void SetDestination(Vector3 destination) {
        _navAgent.SetDestination(destination);
    }

    //enable or disable the navmesh agent
    public void SetAgentStatus(bool status) {
        _navAgent.enabled = status;
    }

    public void SetAcceleration(float acceleration) {
        _navAgent.acceleration = acceleration;
    }

    public void SetSpeed(float speed) {
        _navAgent.speed = speed;
    }

    public bool ReachedDestinaton() {
        return _navAgent.remainingDistance <= 0.1f;
    }
}
