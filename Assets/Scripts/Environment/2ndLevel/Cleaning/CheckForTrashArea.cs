using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CheckForTrashArea : MonoBehaviour {

    public LayerMask trashLayer;
    public float radius;
    private BoxCollider _col;

    public UnityEvent areaChosen; 

    public bool _playerInArea;
    private int trashCount;
    private float _playerInAreaTime;
    public float _timeToChoose;
    private bool _areaChosen;

    // Use this for initialization
    void Start () {
        _col = GetComponent<BoxCollider>();

        _col.size = new Vector3(radius, 3, radius);
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
            Collider[] trash = Physics.OverlapBox(transform.position, new Vector3 (radius, 3, radius), Quaternion.identity ,trashLayer);

            trashCount = trash.Length;

            if (trashCount <= 0)
            {
                Debug.Log(trashCount);
            }
            if (_areaChosen)
            {
                Debug.Log("Invoking");
                areaChosen.Invoke();
            }
        }
    }


    private void ChoosePosition()
    {
        if (_playerInArea)
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
