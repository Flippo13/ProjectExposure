using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class TurbineButton : MonoBehaviour {

    public SpringJoint _springJoint;

    [SerializeField]
    private UnityEvent _turbineButtonEvent; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckButtonForce(); 
	}

    private void CheckButtonForce()
    {
        float pressForce = _springJoint.currentForce.magnitude; 
        print(_springJoint.currentForce);
        print(pressForce);
        if (pressForce > 1)
        {
            Debug.Log("HEY IT WORKS");
            _turbineButtonEvent.Invoke(); 
        }
    }
}
