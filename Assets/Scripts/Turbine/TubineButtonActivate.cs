using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubineButtonActivate : MonoBehaviour {

    private bool _active; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TurbineButton")
        {
            _active = true; 
        }
    }

    

    public bool Active
    {
        get { return _active; }
    }
}
