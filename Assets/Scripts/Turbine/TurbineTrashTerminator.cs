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



    public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == Tags.Hand || other.gameObject.tag == Tags.Player) return;

        if (other.gameObject.layer == Layers.Suckable)
        {
            Destroy(other.gameObject); 
        }
    }
}
