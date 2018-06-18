using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingGlobal : MonoBehaviour {

    public GameObject goalIndicator;
    public GameObject fish;
    public int fishCount;
    public bool playerEntered;
    public GameObject player; 

    public GameObject[] arrayOfFishies;

    public BoxCollider fishTank;
    public Vector3 goal;



	// Use this for initialization
	void Awake () {
        fishTank = GetComponent<BoxCollider>();
        arrayOfFishies = new GameObject[fishCount];

        for (int i = 0; i < arrayOfFishies.Length; i++)
        {
           arrayOfFishies[i] = Instantiate(fish, new Vector3(Random.Range(fishTank.bounds.min.x, fishTank.bounds.max.x), Random.Range(fishTank.bounds.min.y, fishTank.bounds.max.y), Random.Range(fishTank.bounds.min.z, fishTank.bounds.max.z)), Quaternion.Euler(new Vector3(-90, 0, 0)), this.transform); ; 
        }

        goal = new Vector3(Random.Range(fishTank.bounds.min.x, fishTank.bounds.max.x), Random.Range(fishTank.bounds.min.y, fishTank.bounds.max.y), Random.Range(fishTank.bounds.min.z, fishTank.bounds.max.z));
        StartCoroutine("FindNewGoalPosWithDelay", 10.0f);
	}
	
	// Update is called once per frame
	void Update () {
        if(goalIndicator != null)
           goalIndicator.transform.position = goal; 
	}


    IEnumerator FindNewGoalPosWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            NewGoalPos(); 
        }
    }


    private void NewGoalPos()
    {
        goal = new Vector3(Random.Range(fishTank.bounds.min.x, fishTank.bounds.max.x), Random.Range(fishTank.bounds.min.y, fishTank.bounds.max.y), Random.Range(fishTank.bounds.min.z, fishTank.bounds.max.z));
    }


    public Vector3 Goal
    {
        get { return goal; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerEntered = true;
            player = other.gameObject; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerEntered = false; 
            player = null; 
        }
    }

}
