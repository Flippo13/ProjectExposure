using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurbinePieceType; 

public class TurbinePiecePosition : MonoBehaviour {

    public PieceType piece; 

    private bool _inPlacementRange;
    private bool _connected;
    private bool _canConnect; 

    private new Transform transform; 

    private Renderer[] _rend;

   // [HideInInspector]
    public Color notConnectedColor;
   // [HideInInspector]
    public Color connectedColor; 


    void Start()
    {
        transform = GetComponent<Transform>();

        if (transform.childCount == 0)
        {
            _rend = new Renderer[1];
            _rend[0] = GetComponent<Renderer>();
        }
        else
        {
            _rend = new Renderer[transform.childCount];
            _rend = GetComponentsInChildren<Renderer>();
        }
    }

    private void Update()
    {

    }


    public bool InPlacementRange
    {
        get { return _inPlacementRange; }
    }

    public void NotConnectedMaterial()
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.SetColor("_Color", notConnectedColor);
            _rend[i].material.SetColor("_EmissionColor", notConnectedColor);
        }
    }

    public void CanConnectMaterial()
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.SetColor("_Color", connectedColor);
            _rend[i].material.SetColor("_EmissionColor", connectedColor);
        }
    }

    public void Connect()
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.SetColor("_Color",Color.white);
            _rend[i].material.SetColor("_EmissionColor", Color.black);
            _rend[i].material.renderQueue = 1; 
            _connected = true; 
        }
    }

    public bool Connected
    {
        get { return _connected;  }
        set { _connected = value; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.TurbinePiece)
        {
            if (other.GetComponent<TurbinePiece>().piece == piece)
            {
                _inPlacementRange = true; 
                other.GetComponent<TurbinePiece>().SetTurbinePiecePosition(this, this.transform); 
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.TurbinePiece)
        {
            _inPlacementRange = false;
        }
    }
}
