using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour {

    public GameObject cableStart;
    public GameObject cableEnd;
    public GameObject cableNode;
    public Transform cablePartTrans;

    private GameObject cylinder; 

    private Collider _cablePartCollider; 
    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint; 

    private List<GameObject> cableNodesList = new List<GameObject>();
    private List<GameObject> cylinders = new List<GameObject>(); 
    public int nodeAmount; 


    //Info for the Hookes Law Formula F = -K*(|x| - d) (x/|x|) - bv 
    public float springConstant = 0.7f;
    public float desiredDistance;
    public float damping;


    //Cable info
    public float ropeLength;
    public float minRopeLength;
    public float maxRopeLength;
    public float winchSpeed;


    private Vector3 _startPos; 

    // Use this for initialization
    void Awake () {
        _lineRenderer = GetComponent<LineRenderer>();
        _springJoint = cableStart.GetComponent<SpringJoint>();
        SetUpCable();
        SetupCableDisplay(cablePartTrans);
    }

    // Update is called once per frame
    void FixedUpdate () {
        UpdateCable();
    }

    private void SetUpCable()
    {
        cableNodesList.Add(cableStart);
        _startPos = cableEnd.transform.position; 

        float nodeDistance = 1.0f / nodeAmount;
        for (int i = 1; i <= nodeAmount; i++)
        {
            float steps = nodeDistance * i;
            Vector3 nodePos = Vector3.Lerp(cableStart.transform.position, cableEnd.transform.position, steps);
            GameObject realCableNode = Instantiate(cableNode, nodePos, Quaternion.identity, this.transform);
            cableNodesList.Add(realCableNode);
        }
        cableNodesList.Add(cableEnd);
    }

    private void SetupCableDisplay(Transform cylinderPrefab)
    {
       
    }

    private void UpdateCableDisplay(Transform cylinder)
    {
        for (int i = 0; i < cableNodesList.Count; i++)
        {
            Vector3 startPos = cableNodesList[i].transform.position;
            Vector3 endPos = cableNodesList[i + 1].transform.position;

            Vector3 offset = (endPos - startPos) / 2;
        }
    }

    private void UpdateCable()
    {
        for (int i = 0; i < cableNodesList.Count - 1; i++)
        {
            Vector3 distance = cableNodesList[i].transform.position - cableNodesList[i + 1].transform.position;
            Vector3 rVel = cableNodesList[i].GetComponent<Rigidbody>().velocity - cableNodesList[i + 1].GetComponent<Rigidbody>().velocity;

            Vector3 force = -springConstant * (distance.magnitude - desiredDistance) * Vector3.Normalize(distance) - damping * rVel;

            if (i != cableNodesList.Count - 1)
                cableNodesList[i].GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
            if (i + 1 != cableNodesList.Count-1)
            cableNodesList[i + 1].GetComponent<Rigidbody>().AddForce(-force, ForceMode.Force);
        }

        float currentRopeLength = Vector3.Distance(cableStart.transform.position, cableEnd.transform.position);

        if (currentRopeLength > maxRopeLength)
        {
            cableEnd.transform.position = _startPos;

            cableEnd.transform.parent = null; 
        }

        Vector3[] positions = new Vector3[cableNodesList.Count];

        for (int i = 0; i < cableNodesList.Count; i++)
        {
            positions[i] = cableNodesList[i].transform.position;
        }


        _lineRenderer.positionCount = positions.Length;

        _lineRenderer.SetPositions(positions); 
    }

    private void SetUpCableV2()
    {

    }

    private void UpdateCableV2()
    {
        float density = 7750f;

        float radius = 0.02f;
        float volume = Mathf.PI * radius * radius * ropeLength;

        float ropeMass = volume * density;

        ropeMass += cableEnd.GetComponent<Rigidbody>().mass;

        float ropeForce = ropeMass * 9.81f;

        //k is the springconstance
        float k = ropeForce / 0.01f;

        _springJoint.spring = k * 1.0f;
        _springJoint.damper = k * 0.8f;

        _springJoint.maxDistance = ropeLength; 
       
    }

    private void DisplayCable()
    {
        float ropeWidth = 0.2f;

        _lineRenderer.startWidth = ropeWidth;
        _lineRenderer.endWidth = ropeWidth;

        Vector3 A = cableStart.transform.position;
        Vector3 D = cableEnd.transform.position;

        Vector3 B = A + cableStart.transform.up * (-(A - D).magnitude * 0.1f);
        Vector3 C = D + cableEnd.transform.up * ((A - D).magnitude * 0.5f);

       // BezierCurve.GetBezierCurve(A, B, C, D, cableNodesList.transform.position);

        Vector3[] positions = new Vector3[cableNodesList.Count];

        for (int i = 0; i < cableNodesList.Count; i++)
        {
            positions[i] = cableNodesList[i].transform.position; 
        }


        _lineRenderer.positionCount = positions.Length;

        _lineRenderer.SetPositions(positions); 

    }

    private void UpdateWinch()
    {
        bool hasChangedCable = false; 

        if(Input.GetKey(KeyCode.O) && ropeLength < maxRopeLength)
        {
            ropeLength += winchSpeed * Time.deltaTime;

            hasChangedCable = true; 
        }

        if (Input.GetKey(KeyCode.I) && ropeLength > minRopeLength)
        {
            ropeLength -= winchSpeed * Time.deltaTime;

            hasChangedCable = true; 
        }

        if(hasChangedCable)
        {
            ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);

            UpdateCableV2(); 
        }
    }
}
