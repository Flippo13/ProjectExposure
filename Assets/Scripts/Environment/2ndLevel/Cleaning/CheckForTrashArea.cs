using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CheckForTrashArea : MonoBehaviour {


    public LayerMask trashLayer;
    public int minTrashLeft; 
    
   
    private new Transform transform; 
    private BoxCollider _col;
   
    public UnityEvent trashCleared; 

    private int trashCount;
    private bool _playerInArea;

    // Use this for initialization
    void Start () {
        transform = GetComponent<Transform>(); 
        _col = GetComponent<BoxCollider>();
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
            Collider[] trash = Physics.OverlapBox(transform.position, new Vector3(_col.size.x/2, _col.size.y/2, _col.size.z/2),Quaternion.identity,trashLayer);
            trashCount = trash.Length;

            if (trashCount <= minTrashLeft)
            {
                //Do something with the score? 
                trashCleared.Invoke(); 
            }
           
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _playerInArea = true;
            Debug.Log("Player ");
            
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
