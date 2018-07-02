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

    //returns 0 if agent is not rotating, otherwise returns the angle
    public float GetRotationAngle(float treshold) {
        float angle = Vector3.Angle(_navAgent.velocity.normalized, transform.forward);

        if(Mathf.Abs(angle) < treshold) {
            return 0;
        }

        return angle;
    }

    //set destination for the navmesh agent
    public void SetDestination(Vector3 destination) {
        bool result = _navAgent.SetDestination(destination);

        if (!result) Debug.Log("No valid NavMesh destination: " + destination);
    }

    public bool InRange(Vector3 destination, float range) {
        float distance = Vector3.Distance(transform.position, destination);

        return distance < range;
    }

    public bool InRange(Vector3 posA, Vector3 posB, float range) {
        float distance = Vector3.Distance(posA, posB);

        return distance < range;
    }

    public void SetAgentStatus(bool status) {
        _navAgent.enabled = status; 
    }

    public Vector3 GetAgentVelocity() {
        return _navAgent.velocity;
    }
}
