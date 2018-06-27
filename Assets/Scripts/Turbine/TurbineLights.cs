﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineLights : MonoBehaviour {


    public float lightIntensityWhenOn; 

    private Light _light; 


	// Use this for initialization
	void Start () {
        _light = GetComponent<Light>(); 
	}
    
    public void TurnOn()
    {
        _light.intensity = lightIntensityWhenOn; 
    }

}