using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineButtonActivate : MonoBehaviour {

    private bool _active; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "TurbineButton")
        {
            _active = true; 
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "TurbineButton")
        {

            _active = false;
        }
    }
    

    public bool Active
    {
        get { return _active; }
    }
}
