using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour
{
    [SerializeField]
    [EventRef]
    private string _activateSound;
    [SerializeField]
    [EventRef]
    private string _errorSound;

    [SerializeField]
    private GameObject _soundOrigin;

    [SerializeField]
    [EventRef]
    private string _turbineSound;

    [SerializeField]
    private GameObject _button;

    private Animator _anim;
    private bool _trashCleaned;
    private bool _cableConnected;

    private bool _isBeingCalledDown; 
    private bool _activated = false;
    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void Activate()
    {
        Debug.Log("Thank you for giving me a voice Daan! I do make a lot of noise though! ");

        if (_trashCleaned && _cableConnected && !_activated)
        {
            _activated = true;
            _anim.SetBool("enabled", true);
            RuntimeManager.PlayOneShot(_activateSound, _button.transform.position);
            RuntimeManager.PlayOneShot(_turbineSound, _soundOrigin.transform.position);
        }
        else if (_isBeingCalledDown && _cableConnected)
        {
            _anim.SetBool("enabled", true);
            RuntimeManager.PlayOneShot(_activateSound, _button.transform.position);
            RuntimeManager.PlayOneShot(_turbineSound, transform.position);
        }

        /* else
         * display "Turbine is not fixed" Image on Console
        * 
        * 
        */
        else
        {
            Debug.Log("Turbine is not fixed, display that on the console");
            RuntimeManager.PlayOneShot(_errorSound, _button.transform.position);
        }


    }


    public bool CalledDown
    {
       get { return _isBeingCalledDown; }
        set { _isBeingCalledDown = value; }
    }


    public GameObject Button
    {
        get { return _button; }
        set { _button = value; }
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
