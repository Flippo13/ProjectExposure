using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour {


    private Renderer _rend; 

	// Use this for initialization
	void Start () {
        _rend = GetComponentInChildren<Renderer>(); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        _rend.material.color = new Color(0.3f, 0.9f, 0.4f);
    }
}
