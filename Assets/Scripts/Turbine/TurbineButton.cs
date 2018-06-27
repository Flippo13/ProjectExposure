using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurbineButton : MonoBehaviour
{

    
    public SpringJoint springJoint;
    public Collider consoleMeshCol;
    private Collider _col;

    public UnityEvent turbineButtonEvent;

    public TurbineButtonActivate turbineButtonStop;

    private bool _buttonIsBeingPressed;

    private bool _activate;
    // Use this for initialization
    void Start()
    {
        _col = GetComponent<Collider>();
        Physics.IgnoreCollision(_col, consoleMeshCol);
    }

    // Update is called once per frame
    void Update()
    {
        if (turbineButtonStop.Active)
        {
            _activate = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            Debug.Log("Is being Pressed");
            _buttonIsBeingPressed = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            _buttonIsBeingPressed = false;
        }
    }

    public bool Activate
    {
        get { return _activate; }
    }
}
