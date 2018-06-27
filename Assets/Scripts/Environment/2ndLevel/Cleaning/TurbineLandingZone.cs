using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; 
using UnityEngine;

public class TurbineLandingZone : MonoBehaviour {

    public GameObject foundationWithTurbinePrefab;
    public Console console;
    public PowerGrid powerGrid; 

    public UnityEvent areaChosenEvent;
    public float _timeToChoose;

    private BoxCollider _col;
    private Transform foundationTransform;
    private new Transform transform;
    private Turbine _turbine; 

    private float _playerInAreaTime;
    private bool _playerInArea;
    private int trashCount;
    [SerializeField]
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
                _turbine = turbine.GetComponentInChildren<Turbine>();
                _areaChosen = true;
                _playerInAreaTime = 0;
                SetUpTurbine(_turbine);
            }
        }
    }


    private void SetUpTurbine(Turbine turbine)
    {
        turbine.CalledDown = true;
        turbine.Button = console.gameObject; 
        console.turbineButtonEvent.AddListener(turbine.Activate);
        powerGrid.cableConntectedEvent.AddListener(turbine.CableConnected);
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
}
