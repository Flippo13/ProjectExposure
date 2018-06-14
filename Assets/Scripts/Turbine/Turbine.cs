using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour {

    public Transform turbineArea;
    private Animator _anim; 
    public float dropTime;
    public float setHeight; 

    private BoxCollider _col;
    private Vector3 landPos;

    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        //Debug.Log("I still need to have an animation and sound that shows that I work!");
        _anim.SetBool("enabled", true);
    }
}
