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
    private Animator _anim;

    private Vector3 _calculatedCohesion;
    private Vector3 _calculatedSeperation;
    private float speed;
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
        seperateFromPlayerRange = 3;
        StartCoroutine("CalculateDirectionWithDelay", 0.3f);
	}
	
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

        if (Vector3.Distance(transform.position, _fishTank.fishTank.bounds.center) >= _fishTank.fishTank.size.x/2
           || Vector3.Distance(transform.position, _fishTank.fishTank.bounds.center) >= _fishTank.fishTank.size.y/2
           || Vector3.Distance(transform.position, _fishTank.fishTank.bounds.center) >= _fishTank.fishTank.size.z/2)
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
            speed = Random.Range(0.4f, maxSpeed);
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
                rotationSpeed = 0.4f;
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
            //Debug.Log("Neighbours: " + neighbours.Length); 
            _calculatedCohesion = CalculateCohesion(neighbours) + (_fishTank.goal - transform.position);
            _calculatedSeperation = CalculateSeperation(neighbours);
           // Debug.Log("cohesion after: " + _calculatedCohesion);
            //Debug.Log("seperation: " + _calculatedSeperation);

            direction = (_calculatedCohesion + _calculatedSeperation) - transform.position;
            //Debug.Log("direction: " + direction);
        }
    }


        /*
    private Vector3 CalculateCohesionV2()
    {
        Vector3 cohesion = Vector3.zero;
        int neighbourCount = 0;
        float gSpeed = 0;

        GameObject[] neighbours = _fishTank.arrayOfFishies;

        foreach (GameObject neighbour in neighbours)
        {
            if (neighbour != this.gameObject)
            {
                if (Vector3.Distance(neighbour.transform.position, transform.position) < cohesionRange)
                {
                    cohesion += neighbour.transform.position;
                    neighbourCount++;
                    gSpeed += neighbour.GetComponent<FlockingFish>().speed;
                }
            }
        }

        cohesion /= neighbourCount;
        speed = gSpeed / neighbourCount;

        return cohesion;
    }
    
    private Vector3 CalculateSeperationV2()
    {
        Vector3 seperation = Vector3.zero;
        float neighbourCount = 0;
        GameObject[] neighbours = _fishTank.arrayOfFishies;

        foreach (GameObject neighbour in neighbours)
        {
            if (neighbour != this.gameObject)
            {
                if (Vector3.Distance(neighbour.transform.position, transform.position) < seperationRange)
                {
                    seperation += neighbour.transform.position;
                    neighbourCount++;
                }
            }
        }

        seperation /= neighbourCount;

        return seperation;
    }
    */

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


    private Vector3 SeperateFromPlayer(GameObject player)
    {
        Vector3 seperateFromPlayer = Vector3.zero; 

            if (Vector3.Distance(player.transform.position, transform.position) < seperateFromPlayerRange)
            {
                seperateFromPlayer -= this.transform.position - player.transform.position; 
                //seperateModifier = 2;
                speed = 3; 
                Debug.Log("Found Player"); 
            }
            else
            {
               // seperateModifier = 1; 
            }

        return seperateFromPlayer; 
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

      //  Gizmos.DrawWireSphere(transform.position, cohesionRange); 
    }
}
