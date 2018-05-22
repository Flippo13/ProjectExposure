using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalFishFlock : MonoBehaviour 
{

	public GameObject fishPrefab;
	public static int tankSize = 5;
	static int numFish = 10;
	public static GameObject[] allFish = new GameObject[numFish];

	//public static Vector3 goalPos = Vector3.zero; //sets goal pose to middle of tank

	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < numFish; i++)
		{
			Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize),
				Random.Range(-tankSize, tankSize),
			Random.Range(-tankSize, tankSize));

					allFish[i] = (GameObject) Instantiate(fishPrefab, pos, Quaternion.identity);
		}
				
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < allFish.Length; i++) {
			if (Random.Range (0, 10000) < 50) { //every 50 in 10000 times change where goal pose in tank is
			Vector3 goalPos = new Vector3 (Random.Range (-tankSize, tankSize),
			Random.Range (-tankSize, tankSize),
			Random.Range (-tankSize, tankSize));

			allFish[i].transform.position = goalPos;
			}
		}
	//	if (Random.Range (0, 10000) < 50) //every 50 in 10000 times change where goal pose in tank is
	//	{
		//	goalPos = new Vector3 (Random.Range (-tankSize, tankSize),
			//	Random.Range (-tankSize, tankSize),
			//	Random.Range (-tankSize, tankSize));

			//goalPrefab.transform.position = goalPos;
		}
	}

