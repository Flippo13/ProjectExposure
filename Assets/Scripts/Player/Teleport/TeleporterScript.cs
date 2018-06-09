using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    public Transform leftHandAnchor;
    public GameObject indicatorPrefab;
    public GameObject projectilePrefab;

    public int lineResolution;
    public float lineStep;
    public float gravityMultiplier;
    public int maxDistance;

    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    private Material _lineRendererMat;

    private Vector3 _teleportPoint;
    private List<Vector3> _teleportPath;
    private GameObject _indicatorInstance;
    private GameObject _projectileInstance;
    private ProjectileScript _projectileScript;

    private bool _triggerPressed;
    private bool _allowTeleport;

    private TeleportFade _fade;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = true;

        _indicatorInstance = Instantiate(indicatorPrefab);
        _indicatorInstance.SetActive(false);

        _projectileInstance = Instantiate(projectilePrefab);
        _projectileInstance.SetActive(false);

        _projectileScript = _projectileInstance.GetComponent<ProjectileScript>();

        _lineRenderer = leftHandAnchor.GetComponent<LineRenderer>();
        _lineRendererMat = _lineRenderer.material;

        _teleportPath = new List<Vector3>();

        _triggerPressed = false;

        _fade = Camera.main.GetComponent<TeleportFade>();
    }

    public void Update() {
        if(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) >= 0.5f || Input.GetMouseButton(0)) {
            //pressing
            _triggerPressed = true;

            DrawTeleportationLine();
        } else if((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) < 0.5f || Input.GetMouseButtonUp(0)) && _triggerPressed) {
            //teleport
            if (_allowTeleport && !_projectileInstance.activeSelf) TraceTeleportationLine();

            //released
            _triggerPressed = false;
            _allowTeleport = false;

            //disable line renderer 
            _indicatorInstance.SetActive(false);
            _lineRenderer.enabled = false;

            //disable indicator
            _indicatorInstance.SetActive(false);
            _lineRenderer.enabled = false;
        }

        if(_projectileInstance.activeSelf && _projectileScript.IsTeleported()) {
            _fade.StartTeleportFade();
            _projectileScript.SetTeleported(false);
            _projectileInstance.SetActive(false);
        }
    }

    private void DrawTeleportationLine() {
        Vector3[] arcArray = GetArcArray();

        _lineRenderer.enabled = true;

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
                _teleportPath[_teleportPath.Count - 1] = _teleportPoint; //set last teleport point to the list to trace

                Vector3 lookAt = Vector3.Cross(-hit.normal, _indicatorInstance.transform.right); // reverse it if it is down
                lookAt = lookAt.y < 0 ? -lookAt : lookAt; //look at the hits relative up, using the normal as the up vector

                _indicatorInstance.transform.position = hit.point;
                _indicatorInstance.transform.rotation = Quaternion.LookRotation(lookAt, hit.normal); //look at the hits relative up, using the normal as the up vector

                //adjust line renderer
                _lineRenderer.positionCount = _teleportPath.Count;
                _lineRenderer.SetPositions(_teleportPath.ToArray());
                _lineRenderer.SetPosition(_teleportPath.Count - 1, hit.point);

                CheckValidTeleport(hit);

                return; //we found our hit, so we dont need to raycast further
            } else if(i == arcArray.Length - 1) {
                //last position in the array reached and still no target hit

                ray = new Ray(arcArray[i], Vector3.down);

                if(Physics.Raycast(ray, out hit)) {
                    _teleportPoint = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z); //+1 for the player y offset
                    _teleportPath.Add(_teleportPoint); //add last teleport point to the list to trace

                    Vector3 lookAt = Vector3.Cross(-hit.normal, _indicatorInstance.transform.right); // reverse it if it is down
                    lookAt = lookAt.y < 0 ? -lookAt : lookAt; //look at the hits relative up, using the normal as the up vector

                    _indicatorInstance.transform.position = hit.point;
                    _indicatorInstance.transform.rotation = Quaternion.LookRotation(lookAt, hit.normal); //look at the hits relative up, using the normal as the up vector

                    //adjust line renderer and draw a line straight to the ground
                    _lineRenderer.positionCount = lineResolution + 1;
                    _lineRenderer.SetPositions(arcArray);
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
        int layer = hit.collider.gameObject.layer;

        if (layer == Layers.IgnoreTeleport || layer == Layers.Suckable || layer == Layers.CableNode || layer == Layers.Fish) {
            //if we hit a collider that should not allow teleporting
            _lineRendererMat.SetFloat("_Blocked", 1f);
            _indicatorInstance.SetActive(false);

            _allowTeleport = false;
        } else {
            _lineRendererMat.SetFloat("_Blocked", 0f);
            _indicatorInstance.SetActive(true);

            _allowTeleport = true;
        }
    }

    private void TraceTeleportationLine() {
        _projectileInstance.SetActive(true);

        _projectileScript.Trace(_teleportPath, transform);
    }
}