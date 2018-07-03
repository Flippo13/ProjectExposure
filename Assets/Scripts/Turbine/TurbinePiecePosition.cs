using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbinePiecePosition : SuperTurbinePiecePos {

    private bool _inPlacementRange;
    private bool _connected;

    private new Transform transform; 

    private Renderer[] _rend; 

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

    public bool InPlacementRange
    {
        get { return _inPlacementRange; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.TurbinePiece)
        {
            _inPlacementRange = true; 
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.TurbinePiece)
        {
            _inPlacementRange = false;
        }
    }


    public void SetConnectedMaterial(Color newColor)
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.SetColor("_Color", newColor);
            _rend[i].material.SetColor("_EmissionColor", newColor);
        }
    }


    public void Connect()
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.SetColor("_Color",Color.white);
            _rend[i].material.SetColor("_EmissionColor", Color.black);
            _connected = true; 
        }
    }

    public bool Connected
    {
        get { return _connected;  }
        set { _connected = value; }
    }

    public Renderer[] Rend
    {
        get { return _rend; }
    }

}
