using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDrop : MonoBehaviour {

    public float dropHeight;
    public float dropTime;
    public float maxDropSpeed;

    public GameObject turbinePrefab;
    public LayerMask terrainLayer; 
    private new Transform transform;
    private Rigidbody _turbineRB; 
    private bool _calledDown;
    private bool _landed;
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
        transform = GetComponent<Transform>();
        _turbineRB = turbinePrefab.GetComponent<Rigidbody>(); 

        if (_turbineRB.useGravity)
            _turbineRB.useGravity = false;

	}
	
	// Update is called once per frame
	void Update () {
        Drop();
	}


    private void Drop()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up); 
        if(Physics.Raycast(ray,out hit,float.PositiveInfinity,terrainLayer))
        {
            Debug.Log("Going Down " + hit.collider);
            if (hit.collider.tag == "Terrain")
            {
               Vector3 dropPos = new Vector3(hit.point.x, hit.point.y + dropHeight, hit.point.z);
                
                transform.position = Vector3.SmoothDamp(transform.position, dropPos, ref velocity, dropTime, maxDropSpeed);

                if (transform.position.y <= dropPos.y + 2)
                {
                    Landed(); 
                }
            }
        }
    }

    private void Landed()
    {
        if (turbinePrefab != null)
        {
            turbinePrefab.transform.parent = null;
            _turbineRB.useGravity = true;
            Destroy(gameObject);
        }
    }


}
