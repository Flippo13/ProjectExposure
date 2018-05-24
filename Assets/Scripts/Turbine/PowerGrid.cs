using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGrid : MonoBehaviour {

    private BoxCollider _input;
    public GameObject inputBox; 
    private Renderer _rend; 
    private bool _connected; 


	// Use this for initialization
	void Start () {
        _input = GetComponentInChildren<BoxCollider>();
        _rend = GetComponent<Renderer>(); 
	}
	
	// Update is called once per frame
	void Update () {
		if (_connected)
        {
            _rend.material.color = new Color(0.2f, 0.8f, 0.4f, 1f); 
        }
	}

    private void OnCollisionEnter(Collision other)
    {
        print("Collide");
        if (other.collider.tag == "CablePlug")
        {
            other.transform.position = inputBox.transform.position;
            other.transform.parent = null;
            _connected = true; 
        }
                
    }
}
