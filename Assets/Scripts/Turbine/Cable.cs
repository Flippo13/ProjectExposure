using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour {

    public GameObject cableStart;
    public GameObject cableEnd;
    public GameObject cableNode; 

    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint; 

    public List<GameObject> cableNodesList = new List<GameObject>(); 

    //Info for each point of the cable (mass is from the rigidbody)
    private Vector3[] positions;
    private float[] xPositions;
    private float[] yPositions;
    private float[] zPositions; 
    private float[] accelerations;
    private float[] velocitys; 

    //Info for the Hookes Law Formula F = -K*(|x| - d) (x/|x|) - bv 
    public float springConstant = 0.7f;
    public float desiredDistance;
    public float damping;


    //Cable info
    public float ropeLength;
    public float minRopeLength;
    public float maxRopeLength;
    public float winchSpeed;


    // Use this for initialization
    void Awake () {
        _lineRenderer = GetComponent<LineRenderer>();
        _springJoint = cableStart.GetComponent<SpringJoint>();
        // UpdateCableV2(); 
        //cableNodesList.Add(cableStart);
       // cableNodesList.Add(cableEnd);
       // SetUpCable(2);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
         UpdateCable();
       // UpdateWinch(); 
       // DisplayCable(); 
	}

    private void SetUpCable(float pLength)
    {
        int nodeCount = Mathf.RoundToInt(pLength);
        float cableNodes = 1 / pLength;
        positions = new Vector3[nodeCount]; 
        xPositions = new float[nodeCount];
        yPositions = new float[nodeCount];
        zPositions = new float[nodeCount];
        velocitys = new float[nodeCount];
        accelerations = new float[nodeCount]; 

        for (int i = 0; i < nodeCount; i++)
        {
            float steps = cableNodes * i;
            print(steps);
            xPositions[i] = Mathf.Lerp(cableStart.transform.position.x, cableEnd.transform.position.x, steps);
            yPositions[i] = Mathf.Lerp(cableStart.transform.position.y, cableEnd.transform.position.y, steps);
            zPositions[i] = Mathf.Lerp(cableStart.transform.position.z, cableEnd.transform.position.z, steps);
            positions[i] = new Vector3(xPositions[i], yPositions[i], zPositions[i]); 

            //Vector3 nodePos = Vector3.Lerp(cableStart.transform.position, cableEnd.transform.position, steps);

            GameObject realCableNode =  Instantiate(cableNode, positions[i], Quaternion.identity, this.transform);

            cableNodesList.Add(realCableNode); 
        }
    }

    private void UpdateCable()
    {
        for (int i = 0; i < cableNodesList.Count - 1; i++)
        {
            Vector3 distance = cableNodesList[i].transform.position - cableNodesList[i + 1].transform.position;
            Vector3 rVel = cableNodesList[i].GetComponent<Rigidbody>().velocity - cableNodesList[i + 1].GetComponent<Rigidbody>().velocity;

            Vector3 force = -springConstant * (distance.magnitude - desiredDistance) * Vector3.Normalize(distance) - damping * rVel;

//            print("node: " + i + " force " + force);

            if (i != cableNodesList.Count - 1)
                cableNodesList[i].GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
            if (i + 1 != cableNodesList.Count-1)
            cableNodesList[i + 1].GetComponent<Rigidbody>().AddForce(-force, ForceMode.Force);
            //no says anna
        }
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
