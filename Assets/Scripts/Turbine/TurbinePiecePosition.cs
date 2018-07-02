using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbinePiecePosition : MonoBehaviour {

    private bool _inPlacementRange;
    private bool _connected;

    private new Transform transform; 

    private Renderer[] _rend; 

    void Start()
    {
        transform = GetComponent<Transform>();
        Debug.Log("Dede" + transform.childCount); 
        _rend = new Renderer[transform.childCount];

        if (transform.childCount == 0)
        {
            _rend[0] = GetComponent<Renderer>();
        }
        else
        {
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
            Debug.Log("You are in Range"); 
            _inPlacementRange = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Tags.TurbinePiece)
        {
            Debug.Log("You are  not in Range");
            _inPlacementRange = false;
        }
    }

    public void SetConnectedMaterial(Color newColor)
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.color = newColor;
        }
    }

    public void Connect()
    {
        for (int i = 0; i < _rend.Length; i++)
        {
            _rend[i].material.color = Color.white;
            _rend[i].material.SetColor("_EmissionColor", Color.black); 
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
