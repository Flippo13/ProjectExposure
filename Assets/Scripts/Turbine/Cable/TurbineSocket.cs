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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ObjectGrabber>())
        {
            _hand = other.gameObject.GetComponent<ObjectGrabber>();

            if (_rb.isKinematic && other.gameObject == _hand)
            {
                _rb.isKinematic = false;
            }
        }
    }


    public void LetGo()
    {
        _hand.InterruptGrabbing();
    }
}
