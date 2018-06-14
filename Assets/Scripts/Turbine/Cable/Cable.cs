using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour {

    public GameObject cableStart;
    public GameObject cableEnd;
    public GameObject cableNode;

    private GameObject cylinder; 

    private Collider _cablePartCollider; 
    private LineRenderer _lineRenderer;

    private List<GameObject> cableNodesList = new List<GameObject>();
    private List<GameObject> cylinders = new List<GameObject>(); 
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
        SetUpCable();
    }

    // Update is called once per frame
    void FixedUpdate () {
        UpdateCable();
    }


    public void Activate()
    {

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


    private void UpdateCable()
    {
        for (int i = 0; i < cableNodesList.Count - 1; i++)
        {
            Vector3 distance = cableNodesList[i].transform.position - cableNodesList[i + 1].transform.position;
            Vector3 rVel = cableNodesList[i].GetComponent<Rigidbody>().velocity - cableNodesList[i + 1].GetComponent<Rigidbody>().velocity;

            Vector3 force = -springConstant * (distance.magnitude - desiredDistance) * Vector3.Normalize(distance) - damping * rVel;

           
            if(i != cableNodesList.Count - 1)
                cableNodesList[i].GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
            if (i+1 != cableNodesList.Count - 1)
                cableNodesList[i + 1].GetComponent<Rigidbody>().AddForce(-force, ForceMode.Force);
        }

        float currentRopeLength = Vector3.Distance(cableStart.transform.position, cableEnd.transform.position);

        Vector3[] positions = new Vector3[cableNodesList.Count];

        for (int i = 0; i < cableNodesList.Count; i++)
        {
            positions[i] = cableNodesList[i].transform.position;
        }


        _lineRenderer.positionCount = positions.Length;

        _lineRenderer.SetPositions(positions); 
    }

    
}
