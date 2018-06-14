using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class TurbineButton : MonoBehaviour {

    public SpringJoint springJoint;
    public Collider consoleMeshCol;
    private Collider _col; 
    [SerializeField]
    private UnityEvent _turbineButtonEvent;

    public TurbineButtonActivate _turbineButtonPressed; 

	// Use this for initialization
	void Start () {
        _col = GetComponent<Collider>();
        Physics.IgnoreCollision(_col, consoleMeshCol); 
    }
	
	// Update is called once per frame
	void Update () {
	}

   private void OnCollisionStay(Collision other)
   {
        if (other.collider.tag == "Player")
        {
            Debug.Log("Is being Pressed");
            if (_turbineButtonPressed.Active)
            {
                _turbineButtonEvent.Invoke();
                Debug.Log("Invoke Event");
            }
        }
   }


}
