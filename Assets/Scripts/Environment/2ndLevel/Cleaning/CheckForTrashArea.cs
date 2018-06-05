using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CheckForTrashArea : MonoBehaviour {

    public LayerMask trashLayer;
    public float radius;
    private BoxCollider _col;

    public UnityEvent trashCollected; 

    public bool _playerInArea;
    private int trashCount;

	// Use this for initialization
	void Start () {
        _col = GetComponent<BoxCollider>();

        _col.size = new Vector3(radius, 3, radius);
        _col.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
        CheckForTrash();
    }


    private void CheckForTrash()
    {
        if (_playerInArea)
        {
            Collider[] trash = Physics.OverlapBox(transform.position, new Vector3 (radius, 3, radius), Quaternion.identity ,trashLayer);

            trashCount = trash.Length;

            if (trashCount <= 0)
            {
                Debug.Log(trashCount);
                trashCollected.Invoke(); 
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = true; 
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = false;
        }
    }


    public int TrashCount
    {
        get { return trashCount; }
    }

}
