using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionNavigator : MonoBehaviour {

    public float nearSpeed;
    public float nearAcceleration;
    public float farSpeed;
    public float farAcceleration;

    public float switchRadius;

    private NavMeshAgent _navAgent;

    public void Awake() {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    public void CheckForSpeedAdjustment(Vector3 playerPos) {
        Vector3 deltaVec = transform.position - playerPos;

        if(deltaVec.magnitude <= switchRadius) {
            _navAgent.speed = nearSpeed;
            _navAgent.acceleration = nearAcceleration;
        } else {
            _navAgent.speed = farSpeed;
            _navAgent.acceleration = farAcceleration;
        }
    }

    //set destination for the navmesh agent
    public void SetDestination(Vector3 destination) {
        _navAgent.SetDestination(destination);
    }

    public bool ReachedDestinaton() {
        return _navAgent.remainingDistance <= 0.1f;
    }

    public Vector3 GetAgentVelocity() {
        return _navAgent.velocity;
    }
}
