using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

    public float speed;

    private List<Vector3> _tracingPoints;
    private int _pointIndex;
    private TrailRenderer _trail;
    private Transform _player;

    private float _incrementor;
    private float _step;
    private Vector3 _startPos;

    private bool _teleported;

    public void Awake() {
        _trail = GetComponent<TrailRenderer>();
        _trail.enabled = false;
        
        _tracingPoints = null;

        _teleported = false;
    }

    public void Update() {
        FollowPath();
    }

    public void Trace(List<Vector3> points, Transform playerTransform) {
        _tracingPoints = new List<Vector3>(points); //copy the list instead of passing the reference
        _player = playerTransform;
        _trail.enabled = true;

        transform.position = _tracingPoints[0]; //start from the first point in the path
        _pointIndex = 1;

        //initialize first point
        _startPos = _tracingPoints[0];
        Vector3 direction = _tracingPoints[_pointIndex] - _startPos;
        _step = speed / direction.magnitude;
        _incrementor = 0f;
    }

    private void FollowPath() {
        if (_tracingPoints == null || _teleported) return;

        if (_incrementor >= 1f) {
            //next point
            _startPos = _tracingPoints[_pointIndex];
            transform.position = _startPos;
            _pointIndex++;

            if (_pointIndex >= _tracingPoints.Count && !_teleported) {
                //last point in the list
                TeleportPlayer();
                return;
            }

            Vector3 direction = _tracingPoints[_pointIndex] - _startPos;

            _step = speed / direction.magnitude;
            _incrementor = 0;
        } else {
            //linearly interpolate between two points
            _incrementor += _step;
            _incrementor = Mathf.Clamp01(_incrementor);

            transform.position = Vector3.Lerp(_startPos, _tracingPoints[_pointIndex], _incrementor);
        }

    }

    private void TeleportPlayer() {
        //teleport the player and flag the projectile
        _player.position = new Vector3(_tracingPoints[_tracingPoints.Count - 1].x, _tracingPoints[_tracingPoints.Count - 1].y + 1, _tracingPoints[_tracingPoints.Count - 1].z);
        _trail.enabled = false;

        _teleported = true;
    }

    public bool IsTeleported() {
        return _teleported;
    }

    public void SetTeleported(bool status) {
        _teleported = status;
    }
}