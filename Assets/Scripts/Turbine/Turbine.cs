using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour {

    private Animator _anim;
    public TurbineLights[] _lights; 
    private bool _trashCleaned ;
    private bool _cableConnected ;

    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
	}
	
    public void Activate()
    {
        Debug.Log("I still need to have an sound that shows that I work, give me my voice!!");

        if (_trashCleaned && _cableConnected)
        {
            _anim.SetBool("enabled", true);
            //and play an sound
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].TurnOn(); 
            }
        }
        /* else
         * display "Turbine is not fixed" Image on Console
        * 
        * 
        */
        else
            Debug.Log("Turbine is not fixed, display that on the console");

    }


    public void CableConnected()
    {
        _cableConnected = true; 
    }

    public void TrashCleaned()
    {
        _trashCleaned = true; 
    }
}
