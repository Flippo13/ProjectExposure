using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDropCheck : MonoBehaviour {


    private TurbineLandingZone[] _turbineLandingZones; 

	// Use this for initialization
	void Start () {
        _turbineLandingZones = GetComponentsInChildren<TurbineLandingZone>(); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TurbineDropping()
    {
        for (int i = 0; i < _turbineLandingZones.Length; i++)
        {
            Destroy(_turbineLandingZones[i].gameObject);
        }
    }
}
