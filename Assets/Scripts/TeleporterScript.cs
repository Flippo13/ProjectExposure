using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    public Transform leftHandAnchor;
    public Transform rightHandAnchor;

    public int lineResolution;
    public float lineStep;
    public float gravityMultiplier;

    public int maxDistance;

    private Rigidbody _rigidbody;
    private LineRenderer _lineRendererLeft;
    private LineRenderer _lineRendererRight;

    private Ray _ray;
    private Vector3 _teleportPoint;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = true;

        _lineRendererLeft = leftHandAnchor.GetComponent<LineRenderer>();
        _lineRendererRight = rightHandAnchor.GetComponent<LineRenderer>();
    }

    public void Update() {
        //if either A or X is held down
        //if(OVRInput.Get(OVRInput.Button.One)) {
        if (Input.GetMouseButton(1)) {
            DrawTeleportationLine(false); //right
        //} else if(OVRInput.Get(OVRInput.Button.Three)) {
        } else if (Input.GetMouseButton(0)) {
            DrawTeleportationLine(true); //left
        }

        //if either A or X is released
        //if (OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Three)) {
        if (Input.GetMouseButtonUp(0)) {
            Vector3[] linePositions = new Vector3[lineResolution];
            _lineRendererLeft.GetPositions(linePositions);

            Teleport(linePositions);
        } else if(Input.GetMouseButtonUp(1)) {
            Vector3[] linePositions = new Vector3[lineResolution];
            _lineRendererRight.GetPositions(linePositions);

            Teleport(linePositions);
        }
    }

    private void DrawTeleportationLine(bool isLeft) {
        _ray.origin = leftHandAnchor.transform.position;

        if(isLeft) {
            _ray.direction = leftHandAnchor.transform.forward;
            _lineRendererLeft.enabled = true;
            _lineRendererRight.enabled = false;
            _lineRendererLeft.SetPositions(GetArcArray(true));
        } else {
            _ray.direction = rightHandAnchor.transform.forward;
            _lineRendererLeft.enabled = false;
            _lineRendererRight.enabled = true;
            _lineRendererRight.SetPositions(GetArcArray(false));
        }

        /*RaycastHit hit;

        if (Physics.Raycast(_ray, out hit)) {
            Vector3[] linePoints = new Vector3[2];
            linePoints[0] = _ray.origin;
            linePoints[1] = hit.point;

            if (isLeft) {
                _lineRendererLeft.positionCount = 2;
                _lineRendererLeft.SetPositions(linePoints);
            } else {
                _lineRendererLeft.positionCount = 2;
                _lineRendererRight.SetPositions(linePoints);
            }

            _teleportPoint = new Vector3(hit.point.x, hit.point.y + transform.position.y, hit.point.z); //apply offset to stand on the ground

            //need to apply ignore raycast layers to certain objects
        }*/
    }

    private Vector3[] GetArcArray(bool isLeft) {
        Vector3[] vecArray = new Vector3[lineResolution];

        Vector3 startingPos;
        Vector3 lineDirection;

        if(isLeft) {
            startingPos = leftHandAnchor.position;
            lineDirection = leftHandAnchor.transform.forward.normalized;
            _lineRendererLeft.positionCount = lineResolution;
        } else {
            startingPos = rightHandAnchor.position;
            lineDirection = rightHandAnchor.transform.forward.normalized;
            _lineRendererRight.positionCount = lineResolution;
        }

        vecArray[0] = startingPos;

        Vector3 previousPoint = vecArray[0];
        Vector3 velocity = lineDirection;
        Vector3 gravity = new Vector3(0, -0.03f / (float)maxDistance, 0);

        for (int i = 1; i < maxDistance; i++) {
            velocity = velocity + gravity;
            Vector3 currentPoint = previousPoint + velocity;

            vecArray[i] = currentPoint;
            previousPoint = currentPoint;
        }

        velocity = lineDirection * lineStep;
        gravity = new Vector3(0, -1f * gravityMultiplier, 0);
        
        for (int i = maxDistance; i < lineResolution; i++) {
            velocity = velocity + gravity;
            Vector3 currentPoint = previousPoint + velocity;

            vecArray[i] = currentPoint;
            previousPoint = currentPoint;
        }

        return vecArray;
    }

    private void Teleport(Vector3[] linePoints) {
        _lineRendererLeft.enabled = false;
        _lineRendererRight.enabled = false;

        for(int i = 1; i < linePoints.Length; i++) {
            Vector3 deltaVec = linePoints[i] - linePoints[i - 1];
            Ray ray = new Ray(linePoints[i - 1], deltaVec);
            RaycastHit hit;

            //raycast between the line segments
            if(Physics.Raycast(ray, out hit, deltaVec.magnitude)) {
                //teleport
                transform.position = new Vector3(hit.point.x, hit.point.y + transform.position.y, hit.point.z);
                return;
            } else {
                //raycast from the last point down
            }
        }
    }
}
