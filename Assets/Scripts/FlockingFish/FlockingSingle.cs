using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingSingle : MonoBehaviour {

    public float speed;
    public float cohesionRange;
    public float seprationRange;

    private Rigidbody _rb; 

	// Use this for initialization
	void Start () {
        _rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private Vector3 CalculateAlignment()
    {
        Vector3 alignment = Vector3.zero;
        int neighbourCount = 0;

        Collider[] neighbours = Physics.OverlapSphere(transform.position, cohesionRange);

        return alignment;
    }
}
