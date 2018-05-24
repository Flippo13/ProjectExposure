using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereController : MonoBehaviour {

    public float movementSpeed;
    public Transform rotatorZ;
    public Transform hand; 
   // public Text trashCounterUI;
   // public Text objectInRange; 

    private Rigidbody _rigidbody;
    private bool _moving;
    private Collider col; 

    private int _trashCollected;
    private GameObject _turbinePart; 

    void Start () {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        col = GetComponent<Collider>(); 
        _rigidbody = GetComponent<Rigidbody>();

        _moving = false;

        _trashCollected = 0;
	}
	
	public void Update () {
        ProcessInput();
        ProcessMovement();
	}

    public void FixedUpdate() {
        _moving = Input.GetKey(KeyCode.W);
    }

    private void ProcessInput() {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        rotatorZ.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));
        GameObject hitObject = SendOutRay();
        if (hitObject != null)
        {
            if (hitObject.tag == "TurbinePiecePosition")
            {
                print(hitObject.GetComponent<TurbinePiece>().piece);
            }
        }
        //  objectInRange.text = hitObject.name;
        //else
        // objectInRange.text = "Nothing in range"; 
        if (Input.GetMouseButtonDown(0)) {
            //raycast for trash
            if (hitObject != null)
            {
                if (hitObject.tag == "TurbinePart" || hitObject.tag == "CablePlug")
                {
                    _turbinePart = hitObject; 
                    _turbinePart.transform.position = hand.transform.position;
                    _turbinePart.transform.parent = this.transform;
                  //  trashCounterUI.text = "Trash: " + _trashCollected;
                }

                if (hitObject.tag == "TurbinePiecePosition")
                {
                    if (_turbinePart != null)
                    {
                        if (hitObject.GetComponent<TurbinePiece>().piece == _turbinePart.GetComponent<TurbinePiece>().piece)
                        {
                            print("Hello you can place me here!");
                            hitObject.GetComponent<TurbinePiece>().Activate();
                            Destroy(_turbinePart); 
                            //_turbinePart = null; 
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (hitObject != null)
            {
                //if (SendOutRay().tag == "Turbine")
               // {
                   // objectInRange.text = "Fixing Turbine";
                  //  hitObject.GetComponent<Turbine>().Fixing(4); 
               // }
            }
        }
    }

    private void PlaceObject(GameObject objectInHand)
    {
        
    }

    private GameObject SendOutRay()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            if(hit.collider != null)
            {
                return hit.collider.gameObject; 
            }
            return null; 
        }
        return null; 
    }

    private void ProcessMovement() {
        if (!_moving) return;

        _rigidbody.AddForce(Camera.main.transform.forward * movementSpeed);
    }
}
