using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; 
using UnityEngine;

public class Console : MonoBehaviour {
    [SerializeField]
    private TurbineButton _button;

    public UnityEvent turbineButtonEvent;

    // Use this for initialization
    void Start () {
        _button = GetComponentInChildren<TurbineButton>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_button.Activate)
        {
            turbineButtonEvent.Invoke();
            _button.Activate = false; 
        }
    }


    public UnityEvent TurbineButtonEvent
    {
        get { return turbineButtonEvent;  }
    }

}
