using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TeleporterScript : MonoBehaviour {

    public Transform leftHandAnchor;
    public GameObject indicatorPrefab;
    public GameObject projectilePrefab;

    public int lineResolution;
    public float lineStep;
    public float gravityMultiplier;
    public int maxDistance;

    public bool disableLineOnTeleport;

    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    private Material _lineRendererMat;

    private Vector3 _teleportPoint;
    private List<Vector3> _teleportPath;
    private GameObject _indicatorInstance;
    private ProjectileScript _projectileScript;

    private bool _blockedTeleport;
    private bool _allowTeleport;
    private bool _drawIndicator;
    private bool _triggerPressed;

    private TeleportFade _fade;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = true;

        _indicatorInstance = Instantiate(indicatorPrefab);
        _indicatorInstance.SetActive(false);

        _projectileScript = null;

        _lineRenderer = leftHandAnchor.GetComponent<LineRenderer>();
        _lineRendererMat = _lineRenderer.material;

        _teleportPath = new List<Vector3>();

        _blockedTeleport = false;
        _allowTeleport = false;
        _drawIndicator = true;
        _triggerPressed = false;

        _fade = Camera.main.GetComponent<TeleportFade>();
    }

    public void Update() {
        if(disableLineOnTeleport && (OVRInput.GetUp(OVRInput.Button.Three) || Input.GetMouseButtonUp(0))) {
            _drawIndicator = true;
        }

        if(OVRInput.Get(OVRInput.Button.Three) || Input.GetMouseButton(0)) {
            if(disableLineOnTeleport) {
                if (_drawIndicator) DrawTeleportationLine();
            } else {
                DrawTeleportationLine();
            }
            
            _allowTeleport = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.Three) || Input.GetMouseButtonUp(0)) {
            //disable indicator
            _indicatorInstance.SetActive(false);
            _lineRenderer.enabled = false;
            _allowTeleport = false;
        }

        if((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0 && !_triggerPressed) || Input.GetKeyDown(KeyCode.E)) {
            //pressed
            _triggerPressed = true;

            //teleport
            if(disableLineOnTeleport) {
                if (!_blockedTeleport && _allowTeleport && _drawIndicator && _projectileScript == null) {
                    TraceTeleportationLine();

                    //disable line renderer 
                    _indicatorInstance.SetActive(false);
                    _lineRenderer.enabled = false;
                    _allowTeleport = false;
                    if (disableLineOnTeleport) _drawIndicator = false;
                }
            } else {
                if (!_blockedTeleport && _allowTeleport && _projectileScript == null) {
                    TraceTeleportationLine();

                    //disable line renderer 
                    _indicatorInstance.SetActive(false);
                    _lineRenderer.enabled = false;
                    _allowTeleport = false;
                }
            }
        }

        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0 && _triggerPressed) {
            //released
            _triggerPressed = false;
        }

        if(_projectileScript != null && _projectileScript.IsTeleported()) {
            _fade.StartTeleportFade();
            Destroy(_projectileScript.gameObject);
            _projectileScript = null;
        }
    }

    private void DrawTeleportationLine() {
        Vector3[] arcArray = GetArcArray();

        _lineRenderer.enabled = true;
        _lineRenderer.SetPositions(arcArray);

        _teleportPath.Clear();
        _teleportPath.Add(arcArray[0]); // first point

        //bad for performance to raycast that often each frame, think of a better solution
        for (int i = 1; i < arcArray.Length; i++) {
            Vector3 deltaVec = arcArray[i] - arcArray[i - 1];
            Ray ray = new Ray(arcArray[i - 1], deltaVec);
            RaycastHit hit;

            _teleportPath.Add(arcArray[i]); //add point to the list which can be traced by the projectile

            if(Physics.Raycast(ray, out hit, deltaVec.magnitude)) {
                _teleportPoint = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
                _teleportPath.Add(_teleportPoint); //add last teleport point to the list to trace

                _indicatorInstance.transform.position = hit.point;

                CheckValidTeleport(hit);

                return; //we found our hit, so we dont need to raycast further
            } else if(i == arcArray.Length - 1) {
                //last position in the array reachd and still no target hit

                ray = new Ray(arcArray[i], Vector3.down);

                if(Physics.Raycast(ray, out hit)) {
                    _teleportPoint = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z); //+1 for the player y offset
                    _teleportPath.Add(_teleportPoint); //add last teleport point to the list to trace

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
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IgnoreTeleport") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Suckable")) {
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

    private void TraceTeleportationLine() {
        GameObject projectile = Instantiate(projectilePrefab);
        _projectileScript = projectile.GetComponent<ProjectileScript>();

        _projectileScript.Trace(_teleportPath, transform);
    }
}