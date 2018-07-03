using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineTrashTerminator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    private void OnCollisonEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.Suckable)
        {
            Destroy(other.gameObject); 
        }
    }
}
