using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDrop : MonoBehaviour {

    public float dropHeight;
    public float dropTime;
    public float maxDropSpeed;

    public GameObject turbinePrefab;

    private new Transform transform;
    private Collider _col;
    private bool _calledDown;
    private bool _landed;
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
        transform = GetComponent<Transform>();
        _col = GetComponent<Collider>(); 
	}
	
	// Update is called once per frame
	void Update () {
        if (_calledDown)
            Drop(); 
	}


    public void CallDown()
    {
        _calledDown = true; 
    }


    private void Drop()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position,-transform.up, out hit))
        {
            if (hit.collider.tag == "Terrain")
            {
               
               Vector3 dropPos = new Vector3(hit.point.x, hit.point.y + dropHeight, hit.point.z);

                
                transform.position = Vector3.SmoothDamp(transform.position, dropPos, ref velocity, dropTime, maxDropSpeed);

                if (transform.position.y <= dropPos.y + 2)
                {
                    _calledDown = false; 
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
            Destroy(gameObject);
        }
    }


}
