using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    public Transform leftHandAnchor;

    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;

    private Ray _ray;
    private Vector3 _teleportPoint;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.freezeRotation = true;

        _lineRenderer = leftHandAnchor.GetComponent<LineRenderer>();
    }

    public void Update() {
        //if either A or X is held down
        if(OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three)) {
            if (!_lineRenderer.enabled) _lineRenderer.enabled = true;

            _ray.origin = leftHandAnchor.transform.position;
            _ray.direction = leftHandAnchor.transform.forward;

            RaycastHit hit;

            if(Physics.Raycast(_ray, out hit)) {
                Vector3[] linePoints = new Vector3[2];
                linePoints[0] = _ray.origin;
                linePoints[1] = hit.point;

                _lineRenderer.SetPositions(linePoints);
                Debug.Log("Draw lines");

                _teleportPoint = hit.point;
            }
        }

        //if either A or X is released
        if(OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Three)) {
            _lineRenderer.enabled = false;

            transform.position = _teleportPoint;

            Debug.Log("Teleporting");
        }
    }
}
