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

    [SerializeField]
    private TurbineLights[] _lights;

    private TurbinePiecePosition[] _turbinePieces; 

    private bool _isBeingCalledDown;
    private bool _activated = false;
    private bool _piecesConnected;
    private bool _landed;
    private int _pieceIsConnected;

    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (_cableConnected && _activated)
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].TurnOn();
            }
        }
    }

    public void Activate()
    {
        if (_trashCleaned && !_activated)
        {
            Debug.Log("Start Turbine");
            _activated = true;
            _anim.SetBool("enabled", true);
            RuntimeManager.PlayOneShot(_activateSound, _button.transform.position);
            RuntimeManager.PlayOneShot(_turbineSound, _soundOrigin.transform.position);
        }
        else if (_isBeingCalledDown && !_activated)
        {
            _anim.SetBool("enabled", true);
            RuntimeManager.PlayOneShot(_activateSound, _button.transform.position);
            RuntimeManager.PlayOneShot(_turbineSound, transform.position);
        }
        else if (PiecesConnected() != 0 && PiecesConnected() == _turbinePieces.Length && !_activated)
        {
            _anim.SetBool("enabled", true);
            RuntimeManager.PlayOneShot(_activateSound, _button.transform.position);
            RuntimeManager.PlayOneShot(_turbineSound, transform.position);
        }
        else
        {
            Debug.Log("Turbine is not fixed, display that on the console");
            RuntimeManager.PlayOneShot(_errorSound, _button.transform.position);
        }
    }

    private int PiecesConnected()
    {
        _pieceIsConnected = 0;
        for (int i = 0; i < _turbinePieces.Length; i++)
        {
            if(_turbinePieces[i].Connected)
            {
                _pieceIsConnected++; 
            }
        }
        return _pieceIsConnected; 
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

    public TurbineLights[] Lights
    {
        get { return _lights;  }
        set { _lights = value; }
    }

    public TurbinePiecePosition[] Pieces
    {
        get { return _turbinePieces; }
        set { _turbinePieces = value; }
    }
    
    public bool Activated {
        get { return _activated; }
    }


    public bool Landed
    {
        get { return _landed; }
        set { _landed = value; }
    }

}
