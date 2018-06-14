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
    private List<Vector3> _listOfObjectsInRange = new List<Vector3>(); 

    private Vector3 _calculatedCohesion;
    private Vector3 _calculatedSeperation;
    private float speed;
    private Vector3 _goalPos;
    private Vector3 direction;
    private bool _goBack;

    // Use this for initialization
    void Start () {
        speed = Random.Range(0.1f, maxSpeed);
        transform = GetComponent<Transform>();
        _col = GetComponent<SphereCollider>();
        _fishTank = GetComponentInParent<FlockingGlobal>(); 
        StartCoroutine("CalculateDirectionWithDelay", 0.2f);
       // _col.radius = cohesionRange; 
	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

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
        if (Vector3.Distance(transform.position, _fishTank.fishTank.bounds.center) >= _fishTank.fishTank.size.x
           || Vector3.Distance(transform.position, _fishTank.fishTank.bounds.center) >= _fishTank.fishTank.size.y
           || Vector3.Distance(transform.position, _fishTank.fishTank.bounds.center) >= _fishTank.fishTank.size.z)
        {
            _goBack = true;
        }
        else
        {
            _goBack = false;

        }

        if (_goBack)
        {
            direction = _fishTank.fishTank.bounds.center - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            speed = Random.Range(0.6f, maxSpeed);
        }

        else if (Random.Range(0.0f, 10.0f) < 1 && !_goBack)
        {
            Collider[] neighbours = Physics.OverlapSphere(transform.position, cohesionRange, fishLayer);
            _calculatedCohesion = CalculateCohesion(neighbours) + (_fishTank.Goal - transform.position);
            _calculatedSeperation = CalculateSeperation(neighbours);
            //Debug.Log("cohesion: " + _calculatedCohesion);
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
        //Collider[] neighbours = Physics.OverlapSphere(transform.position, cohesionRange, fishLayer);
        foreach (Collider neighbour in neighbours)
        {
            cohesion += neighbour.transform.position;
            neighbourCount++;
            gSpeed += neighbour.GetComponent<FlockingFish>().speed;
        }

        cohesion /= neighbourCount;
        speed = gSpeed / neighbourCount;
        return cohesion; 
    }

    private Vector3 CalculateCohesionV2()
    {
        Vector3 cohesion = Vector3.zero;
        int neighbourCount = 0;
        //float gSpeed = 0;

        
        foreach (Vector3 neighbourPos in _listOfObjectsInRange)
        {
            if (Vector3.Distance(neighbourPos,transform.position) < cohesionRange)
            {
                cohesion += neighbourPos;
                neighbourCount++;
               // gSpeed += neighbourPos.gameObject.GetComponent<FlockingFish>().speed;
            }
        }

        cohesion /= neighbourCount;
        //speed = gSpeed / neighbourCount;
        return cohesion;
    }

    private Vector3 CalculateSeperation(Collider[] neighbours)
    {
        Vector3 seperation = Vector3.zero;
        float neighbourCount = 0;

        //array full off fish that are in range
       // Collider[] neighbours = Physics.OverlapSphere(transform.position, seperationRange, fishLayer);

        foreach (Collider neighbour in neighbours)
        {
            if (Vector3.Distance(neighbour.transform.position, transform.position) < seperationRange)
            {
                seperation -= this.transform.position - neighbour.transform.position;
                neighbourCount++;
            }
        }

        seperation /= neighbourCount;

        return seperation; 
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Fish")
        {
            _listOfObjectsInRange.Add(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Fish")
        {
            _listOfObjectsInRange.Remove(other.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

       // Gizmos.DrawWireSphere(transform.position, cohesionRange); 
    }
}
