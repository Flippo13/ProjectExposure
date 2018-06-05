using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineDrop : MonoBehaviour {

    public float dropHeight;
    public float dropTime;

    public GameObject turbine;

    private new Transform transform;
    private Collider _col;
    private bool _calledDown;
    private bool _landed; 
    private float _dropSpeed;

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
                _dropSpeed += Time.deltaTime / dropTime;
                print("drop speed " + _dropSpeed); 
                Vector3 dropPos = new Vector3(hit.point.x, hit.point.y + dropHeight, hit.point.z);

               transform.position = Vector3.Lerp(transform.position, dropPos, _dropSpeed);

                if (transform.position.y <= dropPos.y + dropHeight + 1)
                {
                    _calledDown = false; 
                    Landed(); 
                }

            }
        }
    }

    private void Landed()
    {
        if (turbine != null)
        {
            turbine.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
