using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineSocket : MonoBehaviour {


    private ObjectGrabber _hand;
    private Rigidbody _rb;
	// Use this for initialization
	void Start () {
        _rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<ObjectGrabber>())
        {
            _hand = other.gameObject.GetComponent<ObjectGrabber>();

            if (_rb.isKinematic)
            {
                _rb.isKinematic = false;
            }
        }
    }

    public void PlugIn()
    {
        _hand.InterruptGrabbing();
    }
}
