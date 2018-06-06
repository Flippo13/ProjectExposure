using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionNavigator : MonoBehaviour {

    private NavMeshAgent _navAgent;

    public void Awake() {
        _navAgent = GetComponent<NavMeshAgent>();
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
