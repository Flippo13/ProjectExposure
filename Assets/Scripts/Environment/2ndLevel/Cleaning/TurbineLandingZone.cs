using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; 
using UnityEngine;

public class TurbineLandingZone : MonoBehaviour {

    public GameObject foundationWithTurbinePrefab;

    public UnityEvent areaChosenEvent;
    public float _timeToChoose;

    private BoxCollider _col;
    private Transform foundationTransform;
    private new Transform transform;
    private Turbine _turbine; 

    private Console _console;
    private PowerGrid _powerGrid;

    private float _playerInAreaTime;
    private bool _playerInArea;
    private int trashCount;
    private bool _areaChosen;

    // Use this for initialization
    void Start() {

        transform = GetComponent<Transform>();
        _col = GetComponent<BoxCollider>();
        _col.isTrigger = true;
        foundationTransform = foundationWithTurbinePrefab.GetComponent<Transform>();

        foundationTransform.position = new Vector3(transform.position.x, foundationTransform.position.y, transform.position.z);

        _playerInArea = false;


    }

    // Update is called once per frame
    void Update() {
        ChoosePosition();
    }


    private void ChoosePosition()
    {
        if (_playerInArea && !_areaChosen)
        {
            if (_playerInAreaTime < _timeToChoose)
            {
                _playerInAreaTime += 1 * Time.deltaTime;
            }
            else
            {
                GameObject turbine =  Instantiate(foundationWithTurbinePrefab, new Vector3(transform.position.x, foundationTransform.position.y, transform.position.z), transform.rotation);
                areaChosenEvent.Invoke();
                _turbine = turbine.GetComponent<Turbine>();

               // if (_powerGrid != null && _console != null)
               // {
              //      _powerGrid.cableConntectedEvent.AddListener(_turbine.CableConnected);
              //      _console.turbineButtonEvent.AddListener(_turbine.Activate);
              //  }

                _areaChosen = true;
                _playerInAreaTime = 0;
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


    public Console Console
    {
        get { return _console;  }
        set { _console = value; }
    }

    public PowerGrid PowerGrid
    {
        get { return _powerGrid;  }
        set { _powerGrid = value; }
    }
}
