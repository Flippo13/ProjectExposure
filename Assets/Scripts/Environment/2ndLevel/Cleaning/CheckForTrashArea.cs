using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CheckForTrashArea : MonoBehaviour {

    public LayerMask trashLayer;
    public int minTrashCleaned; 
    
    public bool turbineArea; 
    private BoxCollider _col;

    public UnityEvent areaChosen;
    public UnityEvent trashCleared; 

    public bool _playerInArea;
    private int trashCount;
    private float _playerInAreaTime;
    public float _timeToChoose;
    private bool _areaChosen;

    // Use this for initialization
    void Start () {
        _col = GetComponent<BoxCollider>();
        _col.isTrigger = true;

        _playerInArea = false;
	}
	
	// Update is called once per frame
	void Update () {
        CheckForTrash();
        ChoosePosition(); 
    }


    private void CheckForTrash()
    {
        if (_playerInArea)
        {
            Collider[] trash = Physics.OverlapBox(transform.position, new Vector3(_col.size.x, _col.size.y, _col.size.z));
            trashCount = trash.Length;

            if (trashCount <= minTrashCleaned)
            {
                //Do something with the score? 
                trashCleared.Invoke(); 
            }
            if (_areaChosen)
            {
                areaChosen.Invoke();
                _areaChosen = false; 
            }
        }
    }


    private void ChoosePosition()
    {
        if (_playerInArea && turbineArea)
        {
            _playerInAreaTime += 1 * Time.deltaTime;
            //Debug.Log("Counting " + _playerInAreaTime);
            if (_playerInAreaTime > _timeToChoose)
            {
                _areaChosen = true; 
            }
        }
        else
        {
            _playerInAreaTime = 0; 
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
