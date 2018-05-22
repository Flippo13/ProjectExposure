using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    public Transform leftHandAnchor;
    public GameObject indicatorPrefab;

    public int lineResolution;
    public float lineStep;
    public float gravityMultiplier;
    public int maxDistance;

    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    private Material _lineRendererMat;

    private Vector3 _teleportPoint;
    private GameObject _indicatorInstance;

    private bool _blockedTeleport;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = true;

        _indicatorInstance = Instantiate(indicatorPrefab);
        _indicatorInstance.SetActive(false);

        _lineRenderer = leftHandAnchor.GetComponent<LineRenderer>();
        _lineRendererMat = _lineRenderer.material;

        _blockedTeleport = false;
    }

    public void Update() {
        if(OVRInput.Get(OVRInput.Button.Three) || Input.GetMouseButton(0)) {
            DrawTeleportationLine();
        }

        if (OVRInput.GetUp(OVRInput.Button.Three) || Input.GetMouseButtonUp(0)) {
            //teleport
            if(!_blockedTeleport) {
                transform.position = _teleportPoint;
            }

            _indicatorInstance.SetActive(false);
            _lineRenderer.enabled = false;
        }
    }

    private void DrawTeleportationLine() {
        Vector3[] arcArray = GetArcArray();

        _lineRenderer.enabled = true;
        _lineRenderer.SetPositions(arcArray);

        //bad for performance to raycast that often each frame, think of a better solution
        for (int i = 1; i < arcArray.Length; i++) {
            Vector3 deltaVec = arcArray[i] - arcArray[i - 1];
            Ray ray = new Ray(arcArray[i - 1], deltaVec);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, deltaVec.magnitude)) {
                _teleportPoint = new Vector3(hit.point.x, hit.point.y + transform.position.y, hit.point.z);
                
                _indicatorInstance.transform.position = hit.point;

                CheckValidTeleport(hit);

                return; //we found our hit, so we dont need to raycast further
            } else if(i == arcArray.Length - 1) {
                //last position in the array reachd and still no target hit

                ray = new Ray(arcArray[i], Vector3.down);

                if(Physics.Raycast(ray, out hit)) {
                    _teleportPoint = new Vector3(hit.point.x, hit.point.y + transform.position.y, hit.point.z);

                    _indicatorInstance.transform.position = hit.point;

                    //adjust line renderer and draw a line straight to the ground
                    _lineRenderer.positionCount = lineResolution + 1;
                    _lineRenderer.SetPosition(lineResolution, hit.point);

                    CheckValidTeleport(hit);
                }
            }
        }
    }

    private Vector3[] GetArcArray() {
        //draw an arc by simulating velocity and gravity for a given amlount of steps
        Vector3[] vecArray = new Vector3[lineResolution];

        Vector3 startingPos = leftHandAnchor.position;
        Vector3 lineDirection = leftHandAnchor.transform.forward.normalized;

        _lineRenderer.positionCount = lineResolution;

        vecArray[0] = startingPos;

        Vector3 previousPoint = vecArray[0];
        Vector3 velocity = lineDirection;
        Vector3 gravity = new Vector3(0, -0.03f / (float)maxDistance, 0);

        //straigther line for the max destance
        for (int i = 1; i < maxDistance; i++) {
            velocity = velocity + gravity;
            Vector3 currentPoint = previousPoint + velocity;

            vecArray[i] = currentPoint;
            previousPoint = currentPoint;
        }

        velocity = lineDirection * lineStep;
        gravity = new Vector3(0, -1f * gravityMultiplier, 0);
        
        //rapid falloff for the rest of the resolution
        for (int i = maxDistance; i < lineResolution; i++) {
            velocity = velocity + gravity;
            Vector3 currentPoint = previousPoint + velocity;

            vecArray[i] = currentPoint;
            previousPoint = currentPoint;
        }

        return vecArray;
    }

    private void CheckValidTeleport(RaycastHit hit) {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IgnoreTeleport")) {
            //if we hit a collider that should not allow teleporting
            _lineRendererMat.SetFloat("_Blocked", 1f);
            _indicatorInstance.SetActive(false);
            _blockedTeleport = true;
        } else {
            _lineRendererMat.SetFloat("_Blocked", 0f);
            _indicatorInstance.SetActive(true);
            _blockedTeleport = false;
        }
    }
}