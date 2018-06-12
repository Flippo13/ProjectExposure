using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbinePiecePosition : MonoBehaviour {


    private bool _inPlacementRange; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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


}
