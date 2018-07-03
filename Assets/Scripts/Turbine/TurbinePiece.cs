using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurbinePieceType; 

public class TurbinePiece : MonoBehaviour {

    public PieceType piece; 

    public float offset; 
    [HideInInspector]
    public TurbinePiecePosition turbinePiecePosition; 

    private new Transform transform;
    private bool _correctXRotation; 
    private bool _correctYRotation;
    private bool _correctZRotation;

    private ObjectGrabber _hand;
    private Transform _turbinePosTransform;

    // Use this for initialization
    void Start () {
        transform = GetComponent<Transform>();
	}

    void Update()
    {
        //CheckRotation(); 
        if (turbinePiecePosition != null)
        {
            if (_hand != null && turbinePiecePosition.InPlacementRange)
            {
                if (piece == PieceType.TurbineBlade)
                {
                    _correctXRotation = AxisIsOne(transform.right, true, false, true);
                    _correctYRotation = AxisIsOne(transform.up, false, true, false);
                    _correctZRotation = AxisIsOne(transform.forward, true, false, true);
                }

                else if (piece == PieceType.TurbinePipe)
                {
                    _correctXRotation = AxisIsOne(transform.right, true, false, false);
                    _correctYRotation = AxisIsOne(transform.up, false, true, false);
                    _correctZRotation = AxisIsOne(transform.forward, false, false, true);
                }

                if (_correctXRotation && _correctYRotation && _correctZRotation)
                {
                    Debug.Log("Stuff here " + _hand.IsHoldingObject());
                    turbinePiecePosition.CanConnectMaterial(); 
                    if (!_hand.IsHoldingObject()) {
                        Connected();
                        Debug.Log("Stuff man");
                    }
                }
                else
                {
                    //Something something
                    turbinePiecePosition.NotConnectedMaterial();
                }
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


    public void Connected()
    {
        turbinePiecePosition.Connected = true;
        _hand.InterruptGrabbing(); 
        _hand = null;
        turbinePiecePosition.Connect(); 
        Destroy(this.gameObject);
    }

    public void SetTurbinePiecePosition(TurbinePiecePosition piecePosition, Transform pieceTransform)
    {
        turbinePiecePosition = piecePosition;
        _turbinePosTransform = pieceTransform; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectGrabber>())
        {
            _hand = other.GetComponent<ObjectGrabber>();

            Debug.Log("Hand is now: " + _hand.name);
        }
    }

}
