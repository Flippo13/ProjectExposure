using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbinePiece : MonoBehaviour {

    public enum Piece {TrubineBlade, TurbineFoundation};
    public Piece piece;

    public GameObject turbinePiecePosition; 



    public bool connected;

    public float offset; 

    private Renderer rend;
    private new Transform transform;
    private Transform _turbinePosTransform;
    private TurbinePiecePosition _turbinePiecePosition; 
    public bool grabbed;
    private bool _correctXRotation; 
    private bool _correctYRotation;
    private bool _correctZRotation; 
    // Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        _turbinePosTransform = turbinePiecePosition.GetComponent<Transform>();
        _turbinePiecePosition = _turbinePosTransform.GetComponent<TurbinePiecePosition>(); 
		if(this.tag == "TurbinePiecePosition")
        {
            rend.material.color = new Color(0.3f, 0.8f,0.4f, 0.21f);
            connected = false; 
        }

       
	}
	
	// Update is called once per frame
	void Update () {
        //CheckRotation(); 
        if (this.tag != "TurbinePiecePosition" && grabbed && _turbinePiecePosition.InPlacementRange)
        {
            if (piece == Piece.TrubineBlade)
            {
                _correctXRotation = AxisIsOne(transform.right, true, false, true);
                _correctYRotation = AxisIsOne(transform.up, false, true, true);
                _correctZRotation = AxisIsOne(transform.forward, true, false, true);


                if (_correctXRotation && _correctYRotation && _correctZRotation)
                {
                    connected = true;
                    _turbinePosTransform.GetComponent<Renderer>().material.renderQueue = 1;
                    _turbinePosTransform.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f); 
                    grabbed = false;
                    Connected(); 
                    Debug.Log("Can Connect");
                }
            }
        }
    }


    private void CheckRotation()
    {
        /*
        float dotOfXtoY = Vector3.Dot(transform.right, _turbinePosTransform.transform.up);
        float dotOfXtoX = Vector3.Dot(transform.right, _turbinePosTransform.transform.right);
        float dotOfXtoZ = Vector3.Dot(transform.right, _turbinePosTransform.transform.forward);

        float dotOfYtoY = Vector3.Dot(transform.up, _turbinePosTransform.transform.up);
        float dotOfYtoX = Vector3.Dot(transform.up, _turbinePosTransform.transform.right);
        float dotOfYtoZ = Vector3.Dot(transform.up, _turbinePosTransform.transform.forward);

        float dotOfZtoY = Vector3.Dot(transform.forward, _turbinePosTransform.transform.up);
        float dotOfZtoX = Vector3.Dot(transform.forward, _turbinePosTransform.transform.right);
        float dotOfZtoZ = Vector3.Dot(transform.forward, _turbinePosTransform.transform.forward);

        Debug.Log("Y to Y: " + dotOfYtoY);
        Debug.Log("Y to X: " + dotOfYtoX);
        Debug.Log("Y to Z: " + dotOfYtoZ);

        Debug.Log("Y +- other Y: " + (1 <= dotOfYtoY + offset || -1 >= dotOfYtoY - offset));
        Debug.Log("Y +- other X: " + (1 <= dotOfYtoX + offset || -1 >= dotOfYtoX - offset));
        Debug.Log("Y +- other Z: " + (1 <= dotOfYtoZ + offset || -1 >= dotOfYtoZ - offset));
        */
    }

    private bool AxisIsOne(Vector3 turbinePieceAxis, bool compareToX, bool compareToY, bool compareToZ)
    {
        float dotOfAxisToX = Vector3.Dot(turbinePieceAxis, _turbinePosTransform.right);
        float dotOfAxisToY = Vector3.Dot(turbinePieceAxis, _turbinePosTransform.up); 
        float dotOfAxisToZ = Vector3.Dot(turbinePieceAxis, _turbinePosTransform.forward);

        if (compareToX && 1 <= dotOfAxisToX + offset || -1 >= dotOfAxisToX - offset)
            return true;
        else if (compareToY && 1 <= dotOfAxisToY + offset || -1 >= dotOfAxisToY - offset && compareToY)
            return true; 
        else if (compareToZ && 1 <= dotOfAxisToZ + offset || -1 >= dotOfAxisToZ - offset)
            return true;
        else
            return false; 
        
    }


    public void Activate()
    {
        rend.material.color = new Color(0.3f, 0.8f, 0.4f, 1.0f);
        connected = true; 
    }

    public void Connected()
    {
        Destroy(this.gameObject); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectGrabber>())
        {
            grabbed = true; 
        }
    }

}
