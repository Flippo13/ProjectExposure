using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour {

    private Animator _anim; 

    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
	}
	
    public void Activate()
    {
        //Debug.Log("I still need to have an animation and sound that shows that I work!");
        _anim.SetBool("enabled", true);
    }
}
