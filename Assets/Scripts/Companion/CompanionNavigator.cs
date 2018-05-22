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

    //enable or disable the navmesh agent
    public void SetAgentStatus(bool status) {
        _navAgent.enabled = status;
    }
}
