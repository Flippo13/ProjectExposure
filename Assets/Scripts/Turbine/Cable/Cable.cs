using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cable : MonoBehaviour {

    public GameObject cableStart;
    public GameObject cableEnd;
    public GameObject cableNode;

    private GameObject cylinder;
    private TurbineSocket _socket; 
    private Collider _cablePartCollider; 
    private LineRenderer _lineRenderer;

    private List<GameObject> cableNodesList = new List<GameObject>();
    private List<GameObject> cylinders = new List<GameObject>();
    private float[] distanceBetweenNodes;
    private float _maximumLength;
    private float _currentLength; 
   // private float currentLength; 
    public int nodeAmount; 

    //Info for the Hookes Law Formula F = -K*(|x| - d) (x/|x|) - bv 
    public float springConstant = 0.7f;
    public float desiredDistance;
    public float damping;

    //Cable info
    private Vector3 nodeStartPos; 

    private Vector3 _startPos; 

    // Use this for initialization
    void Awake () {
        _lineRenderer = GetComponent<LineRenderer>();
        _socket = cableEnd.GetComponent<TurbineSocket>(); 
        SetUpCable();
    }

    // Update is called once per frame
    void FixedUpdate () {
        UpdateCable();
        DisplayCable();
    }

    //Do we need to disable the cable at some point? 
    public void Activate()
    {

    }


    private void SetUpCable()
    {
        cableNodesList.Add(cableStart);
        _startPos = cableEnd.transform.position;

        distanceBetweenNodes = new float[nodeAmount + 1];
        _maximumLength = (nodeAmount + 1) * desiredDistance;
        Debug.Log("maximum distance: " + _maximumLength); 

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


    private void UpdateCable()
    {
        for (int i = 0; i < cableNodesList.Count - 1; i++)
        {
            Vector3 distance = cableNodesList[i].transform.position - cableNodesList[i + 1].transform.position;
            Vector3 rVel = cableNodesList[i].GetComponent<Rigidbody>().velocity - cableNodesList[i + 1].GetComponent<Rigidbody>().velocity;

            Vector3 force = -springConstant * (distance.magnitude - desiredDistance) * Vector3.Normalize(distance) - damping * rVel;

            cableNodesList[i].transform.LookAt(cableNodesList[i + 1].transform);
            cableNodesList[i].GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
            cableNodesList[i + 1].GetComponent<Rigidbody>().AddForce(-force, ForceMode.Force);
        }
              

        for (int i = 0; i < distanceBetweenNodes.Length; i++)
        {
            distanceBetweenNodes[i] = Vector3.Distance(cableNodesList[i].transform.position, cableNodesList[i + 1].transform.position);

            _currentLength += distanceBetweenNodes[i];
        }

        if (_currentLength >= _maximumLength)
        {
            _socket.LetGo(); 
        }
    }


    private float GetCurrentLength()
    {
        float currentLength = 0;

        for (int i = 0; i < distanceBetweenNodes.Length; i++)
        {
            currentLength += distanceBetweenNodes[i];
        }

        return currentLength; 
    }


    private void DisplayCable()
    {
        Vector3[] positions = new Vector3[cableNodesList.Count];

        for (int i = 0; i < cableNodesList.Count; i++)
        {
            positions[i] = cableNodesList[i].transform.position;
        }

        _lineRenderer.positionCount = positions.Length;

        _lineRenderer.SetPositions(positions); 

    }
}
