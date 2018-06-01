using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingFish : MonoBehaviour {

    public LayerMask fishLayer; 
    public float cohesionRange;
    public float seperationRange; 
    public float maxSpeed;
    public float rotationSpeed; 

    private Vector3 _calculatedCohesion;
    private Vector3 _calculatedSeperation;
    private float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

        if (Random.Range(0, 5) < 1)
        {
            _calculatedCohesion = CalculateCohesion();
            _calculatedSeperation = CalculateSeperation();

            Vector3 direction = (_calculatedCohesion + _calculatedSeperation)- transform.position; 
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            }   
        }
	}

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesion = Vector3.zero;
        int neighbourCount = 0;

        //array full off fish that are in range
        Collider[] neighbours = Physics.OverlapSphere(transform.position, cohesionRange, fishLayer);

        foreach (Collider neighbour in neighbours)
        {
            cohesion += neighbour.transform.position;
            neighbourCount++; 
        }

        cohesion /= neighbourCount; 

        return cohesion; 
    }

    private Vector3 CalculateSeperation()
    {
        Vector3 seperation = Vector3.zero;
        float neighbourCount = 0;

        //array full off fish that are in range
        Collider[] neighbours = Physics.OverlapSphere(transform.position, seperationRange, fishLayer);

        foreach (Collider neighbour in neighbours)
        {
            seperation -= this.transform.position - neighbour.transform.position;
            neighbourCount++;
        }

        return seperation; 
    }
}
