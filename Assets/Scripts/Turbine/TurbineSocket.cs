using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineSocket : MonoBehaviour {


    private ObjectGrabber _hand; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hello Hand");
        if (other.gameObject.GetComponent<ObjectGrabber>())
        {
            _hand = other.gameObject.GetComponent<ObjectGrabber>();
            Debug.Log(_hand.name);
        }
    }

    public void PlugIn()
    {
        _hand.InterruptGrabbing();
    }
}
