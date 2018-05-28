using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingFishies : MonoBehaviour {

    public List<GameObject> neighbours = new List<GameObject>();

    public float speed;
    private int _index; 
    // Use this for initialization
    void Start () {
        // neighbourCount = 0; 
        
           //this.GetComponent<Rigidbody>().velocity = new Vector3(1, 0, 0); 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach (GameObject agent in neighbours)
        {
            Vector3 alignment = CalculateAlignment(agent);
            Vector3 cohesion = CalculateCohesion(agent);
            Vector3 seperation = CalculateSeperation(agent); 

            agent.GetComponent<Rigidbody>().velocity += alignment + cohesion + seperation + new Vector3(10,0,0);

            Vector3.Normalize(agent.GetComponent<Rigidbody>().velocity);

            agent.GetComponent<Rigidbody>().velocity *= speed;
            
            //print(_index); 
        } 
	}

    private Vector3 CalculateAlignment(GameObject myAgent)
    {
        Vector3 alignment = new Vector3(0,0,0); 
        int neighbourCount = 0;

        foreach (GameObject agent in neighbours)
        {
            if (agent != myAgent)
            {
                if (Vector3.Distance(agent.transform.position, myAgent.transform.position) < 10)
                {
                    alignment += agent.GetComponent<Rigidbody>().velocity;
                    neighbourCount++; 
                }
            }
        }

        if (neighbourCount == 0)
            return alignment;


        alignment.x /= neighbourCount;
        alignment.y /= neighbourCount;
        alignment.z /= neighbourCount;
        Vector3.Normalize(alignment);
        //print(alignment); 
        return alignment; 
    }

    private Vector3 CalculateCohesion(GameObject myAgent)
    {
        Vector3 cohesion = new Vector3(0, 0, 0);

        int neighbourCount = 0;

        foreach (GameObject agent in neighbours)
        {
            if (agent != myAgent)
            {
                if (Vector3.Distance(agent.transform.position, myAgent.transform.position) < 25)
                {
                    cohesion += agent.transform.position;
                    neighbourCount++;
                }
            }
        }

        if (neighbourCount == 0)
            return cohesion;

        cohesion.x /= neighbourCount;
        cohesion.y /= neighbourCount;
        cohesion.z /= neighbourCount;

        cohesion = new Vector3(cohesion.x - myAgent.transform.position.x, cohesion.y - myAgent.transform.position.y, cohesion.z - myAgent.transform.position.z); 

        Vector3.Normalize(cohesion);
       // print("cohesion " + cohesion);

        return cohesion; 
    }

    private Vector3 CalculateSeperation(GameObject myAgent)
    {
        Vector3 seperation = new Vector3(0, 0, 0);

        int neighbourCount = 0;

        foreach (GameObject agent in neighbours)
        {
            if (agent != myAgent)
            {
                if (Vector3.Distance(agent.transform.position, myAgent.transform.position) < 2)
                {
                    seperation += agent.transform.position - myAgent.transform.position;
                    neighbourCount++;
                }
            }
        }

        if (neighbourCount == 0)
            return seperation;

        seperation.x /= neighbourCount;
        seperation.y /= neighbourCount;
        seperation.z /= neighbourCount;

        seperation *= -1; 

        Vector3.Normalize(seperation);
        //print("seperation " + seperation);
        return seperation; 
    }

}
