using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumArea : MonoBehaviour {

    private CapsuleCollider col;
    public LayerMask suckableLayer;

    [HideInInspector]
    public List<Transform> suckableObjectsList = new List<Transform>(); 


	// Use this for initialization
	void Start () {
        col = GetComponent<CapsuleCollider>();
        StartCoroutine("CheckSuckableObjectWithDelay", 0.2f); 
	}
	
    IEnumerator CheckSuckableObjectWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            ObjectsInArea(); 
        }
    }

    public void ObjectsInArea()
    {
        suckableObjectsList.Clear();

        Vector3 colliderStart = col.bounds.min;
        Vector3 colliderEnd = col.bounds.max;
        float colliderRaduis = col.radius;
        //print("colliderStart " + colliderStart);
        //print("colliderEnd " + colliderEnd); 
        Collider[] objectsInArea = Physics.OverlapCapsule(colliderStart, colliderEnd, colliderRaduis,suckableLayer);
        for (int i = 0; i < objectsInArea.Length; i++)
        {
            Transform suckable = objectsInArea[i].transform; 
            suckableObjectsList.Add(suckable); 
        }
    }

}
