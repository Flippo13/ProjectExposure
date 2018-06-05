using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour {

    public Transform turbineArea;
    public float dropTime;
    public float setHeight; 
    private Renderer _rend;

    private BoxCollider _col;
    private Vector3 landPos;

    // Use this for initialization
    void Start () {
        _rend = GetComponentInChildren<Renderer>();
        _col = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        Debug.Log("I still need to have an animation and sound that shows that I work!");
    }
}
