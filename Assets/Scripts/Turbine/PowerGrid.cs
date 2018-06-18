using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGrid : MonoBehaviour {

    private Renderer _rend; 
    private bool _connected; 


	// Use this for initialization
	void Start () {
        _rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_connected)
        {
            _rend.material.color = new Color(0.2f, 0.8f, 0.4f, 1f); 
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.CablePlug)
        {
            other.GetComponent<TurbineSocket>().LetGo();
            other.transform.position = this.transform.position;
            other.transform.rotation = this.transform.rotation;
            _connected = true;

            other.GetComponent<Rigidbody>().isKinematic = true;
        }
                
    }

    public bool Connected
    {
        get { return _connected; }
    }
}
