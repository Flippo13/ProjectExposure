using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbinePiece : MonoBehaviour {

    public enum Piece {TrubineBlade, TurbineFoundation};
    public Piece piece;

    public Color canConnectedColor;
    public Color notConnectedColor; 

    public GameObject turbinePiecePosition; 

    public bool connected;

    public float offset; 

    private new Transform transform;
    private Transform _turbinePosTransform;
    private TurbinePiecePosition _turbinePiecePosition; 
    private bool _correctXRotation; 
    private bool _correctYRotation;
    private bool _correctZRotation;

    private ObjectGrabber _hand; 

    // Use this for initialization
	void Start () {
        transform = GetComponent<Transform>();
        _turbinePosTransform = turbinePiecePosition.GetComponent<Transform>();
        _turbinePiecePosition = _turbinePosTransform.GetComponent<TurbinePiecePosition>(); 

		if(this.tag == "TurbinePiecePosition")
        {
            connected = false; 
        }

        _turbinePiecePosition.SetConnectedMaterial(notConnectedColor); 
	}

    void Update()
    {
        //CheckRotation(); 
        if (this.tag != "TurbinePiecePosition" && _turbinePiecePosition.InPlacementRange)
        {
            if (piece == Piece.TrubineBlade)
            {
                _correctXRotation = AxisIsOne(transform.right, true, false, true);
                _correctYRotation = AxisIsOne(transform.up, false, true, true);
                _correctZRotation = AxisIsOne(transform.forward, true, false, true);
            }

            else if (piece == Piece.TurbineFoundation)
            {
                _correctXRotation = AxisIsOne(transform.right, true, false, false);
                _correctYRotation = AxisIsOne(transform.up, false, true, false);
                _correctZRotation = AxisIsOne(transform.forward, false, false, true);
            }

            if (_correctXRotation && _correctYRotation && _correctZRotation)
            {
                _turbinePiecePosition.SetConnectedMaterial(canConnectedColor);

                if (!_hand.IsHoldingObject())
                Connected();
            }
            else
            {
                _turbinePiecePosition.SetConnectedMaterial(notConnectedColor);

            }
        }
    }


    

    private bool AxisIsOne(Vector3 turbinePieceAxis, bool compareToX, bool compareToY, bool compareToZ)
    {
        float dotOfAxisToX = Vector3.Dot(turbinePieceAxis, _turbinePosTransform.right);
        float dotOfAxisToY = Vector3.Dot(turbinePieceAxis, _turbinePosTransform.up); 
        float dotOfAxisToZ = Vector3.Dot(turbinePieceAxis, _turbinePosTransform.forward);

        if (compareToX && 1 <= dotOfAxisToX + offset || -1 >= dotOfAxisToX - offset)
            return true;
        else if (compareToY && 1 <= dotOfAxisToY + offset || -1 >= dotOfAxisToY - offset)
            return true; 
        else if (compareToZ && 1 <= dotOfAxisToZ + offset || -1 >= dotOfAxisToZ - offset)
            return true;
        else
            return false; 
        
    }


    public void Activate()
    {
        connected = true; 
    }

    public void Connected()
    {
        // _turbinePosTransform.GetComponent<Renderer>().material.renderQueue = 1;
        // _turbinePosTransform.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
      

        _turbinePiecePosition.Connected = true;
        _hand = null;
        _turbinePiecePosition.Connect(); 
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectGrabber>() && _hand == null)
        {
            
            _hand = other.GetComponent<ObjectGrabber>(); 
        }
    }

}
