using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingFish : MonoBehaviour {

    public LayerMask fishLayer; 
    public float cohesionRange;
    public float seperationRange;
    public float maxSpeed;
    public float rotationSpeed;

    private FlockingGlobal _fishTank; 
    private new Transform transform;
    private SphereCollider _col; 
    private Animator _anim;

    private Vector3 _calculatedCohesion;
    private Vector3 _calculatedSeperation;
    private float speed;
    private float setRotSpeed; 
    private float seperateFromPlayerRange; 
    private bool _swimmingFromPlayer;
    private Vector3 _goalPos;
    private Vector3 direction;
    private bool _goBack;
    private int seperateModifier;

    // Use this for initialization
    void Start () {
        speed = Random.Range(0.1f, maxSpeed);
        transform = GetComponent<Transform>();
        _col = GetComponent<SphereCollider>();
        _anim = GetComponent<Animator>(); 
        _fishTank = GetComponentInParent<FlockingGlobal>();
        setRotSpeed = rotationSpeed; 
        seperateFromPlayerRange = 3;
        StartCoroutine("CalculateDirectionWithDelay", 0.3f);
	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        _anim.speed = speed * 1.2f;

        if (transform.localPosition.x > _fishTank.fishTank.bounds.size.x
            || transform.localPosition.y > _fishTank.fishTank.bounds.size.y
            || transform.localPosition.z > _fishTank.fishTank.bounds.size.z )
        {
            _goBack = true;
        }
        else
        {
            _goBack = false;

        }

        if (_goBack && !_fishTank.playerEntered)
        {
            direction = _fishTank.fishTank.bounds.center - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            speed = Random.Range(0.3f, maxSpeed);
        }

        if (_fishTank.playerEntered && _fishTank.player != null)
        {
            if (Vector3.Distance(_fishTank.player.transform.position, transform.position) < seperateFromPlayerRange)
            {
                direction = _fishTank.player.transform.position - transform.position;
                rotationSpeed = 1; 
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-direction), rotationSpeed * Time.deltaTime);
                speed = 2.2f;
                _swimmingFromPlayer = true; 
            }
            else
            {
                rotationSpeed = setRotSpeed; 
                _swimmingFromPlayer = false;
            }
        }

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
    }


    private IEnumerator CalculateDirectionWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            CalculateDirection(); 
        }
    }


    private void CalculateDirection()
    {
        if (Random.Range(0.0f, 7.0f) < 1 && !_goBack && !_swimmingFromPlayer)
        {
             Collider[] neighbours = Physics.OverlapSphere(transform.position, cohesionRange, fishLayer);
            _calculatedCohesion = CalculateCohesion(neighbours) + (_fishTank.goal - transform.position);
            _calculatedSeperation = CalculateSeperation(neighbours);
            //Debug.Log("seperation: " + _calculatedSeperation);

            direction = (_calculatedCohesion + _calculatedSeperation) - transform.position;
            //Debug.Log("direction: " + direction);
        }
    }


    private Vector3 CalculateCohesion(Collider[] neighbours)
    {
        Vector3 cohesion = Vector3.zero;
        int neighbourCount = 0;
        float gSpeed = 0;

        //array full off fish that are in range
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != this.gameObject && neighbours[i].tag == "Fish")
            {
                cohesion += neighbours[i].transform.position;
                neighbourCount++;
                gSpeed += neighbours[i].GetComponent<FlockingFish>().speed;
            }
        }

        cohesion /= neighbourCount;
        speed = gSpeed / neighbourCount;
        return cohesion; 
    }


    private Vector3 CalculateSeperation(Collider[] neighbours)
    {
        Vector3 seperation = Vector3.zero;
        float neighbourCount = 0;

        //array full off fish that are in range
        // Collider[] neighbours = Physics.OverlapSphere(transform.position, seperationRange, fishLayer);

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != this.gameObject)
            {

                if (Vector3.Distance(neighbours[i].transform.position, transform.position) < seperationRange)
                {
                    seperation -= this.transform.position - neighbours[i].transform.position;
                    neighbourCount++;
                }
            }
        }
        

        seperation /= neighbourCount;

        return seperation; 
    }
}
