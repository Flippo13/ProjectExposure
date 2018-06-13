using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingFish : MonoBehaviour {

    public LayerMask fishLayer; 
    public float cohesionRange;
    public float seperationRange; 
    public float maxSpeed;
    public float rotationSpeed;

    private new Transform transform; 

    private Vector3 _calculatedCohesion;
    private Vector3 _calculatedSeperation;
    private float speed;
    private Vector3 _goalPos; 

	// Use this for initialization
	void Start () {
        speed = Random.Range(0.1f, maxSpeed);
        transform = GetComponent<Transform>(); 
	}
	
	// Update is called once per frame
	void Update () {
        return;

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

        if (Random.Range(0, 3) < 1)
        {
            _calculatedCohesion = CalculateCohesion() + (FlockingGlobal.goal - transform.position);
            _calculatedSeperation = CalculateSeperation();
            Debug.Log("cohesion: " + _calculatedCohesion);
            Debug.Log("seperation: " + _calculatedSeperation);

            Vector3 direction = (_calculatedCohesion + _calculatedSeperation) - transform.position;
            Debug.Log("direction: " + direction);
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
        float gSpeed = 0;

        //array full off fish that are in range
        Collider[] neighbours = Physics.OverlapSphere(transform.position, cohesionRange, fishLayer);
        Debug.Log("neighbours: " + neighbours.Length);
        foreach (Collider neighbour in neighbours)
        {
            cohesion += neighbour.transform.position;
            neighbourCount++;
            gSpeed += neighbour.GetComponent<FlockingFish>().speed;
        }

        cohesion /= neighbourCount;
        speed = gSpeed / neighbourCount;
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

        seperation /= neighbourCount;

        return seperation; 
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

       // Gizmos.DrawWireSphere(transform.position, cohesionRange); 
    }
}
