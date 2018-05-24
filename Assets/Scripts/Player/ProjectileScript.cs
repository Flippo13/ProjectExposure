using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

    public float speed;

    private List<Vector3> _tracingPoints;
    private int _pointIndex;
    private TrailRenderer _trail;
    private Transform _player;

    public void Awake() {
        _trail = GetComponent<TrailRenderer>();
        
        _tracingPoints = null;
        _pointIndex = 0;
    }

    public void Update() {
        FollowPath();
    }

    public void Trace(List<Vector3> points, Transform playerTransform) {
        _tracingPoints = new List<Vector3>(points); //should copy the list instead of passing the reference
        _player = playerTransform;

        transform.position = _tracingPoints[0]; //start from the first point in the path
        _pointIndex = 1;
    }

    private void FollowPath() {
        if (_tracingPoints == null || _pointIndex >= _tracingPoints.Count) return;

        Vector3 direction = _tracingPoints[_pointIndex] - transform.position;

        if (direction.magnitude > 0.1f) {
            transform.Translate(direction.normalized * speed);
        } else if (_pointIndex == _tracingPoints.Count - 1) {
            //last point
            TeleportPlayer(_pointIndex);
        } else {
            //adjust path if you overshoot and trace the next point
            transform.position = _tracingPoints[_pointIndex];
            _pointIndex++; //next point
        }
   
    }

    private void TeleportPlayer(int pointIndex) {
        //teleport the player and destroy the projectile
        _player.position = _tracingPoints[_tracingPoints.Count - 1];
        _trail.enabled = false;
        Destroy(gameObject);
    }
}