using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingBellSound : MonoBehaviour
{



    [SerializeField]
    [FMODUnity.EventRef]
    private string _doorOpen;

    [SerializeField]
    [FMODUnity.EventRef]
    private string _doorClose;

    [SerializeField]
    [FMODUnity.EventRef]
    private string _divingBellMove;


    [SerializeField]
    [FMODUnity.EventRef]
    private string _landSound;

    private FMOD.Studio.EventInstance _moveSound;
    private bool _moving = false;

    public void DoorSound(string open)
    {
        if (open == "true")
        {
            RuntimeManager.PlayOneShot(_doorOpen, transform.GetChild(0).transform.position);
        }
        else
            RuntimeManager.PlayOneShot(_doorClose, transform.GetChild(0).transform.position);
    }


    public void MoveSound()
    {
        _moveSound = FMODUnity.RuntimeManager.CreateInstance(_divingBellMove);
        _moving = true;
        _moveSound.start();
    }

    private void Update()
    {
        if (_moving)
            _moveSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.GetChild(0).transform));
    }

    public void LandSound()
    {
        _moveSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.PlayOneShot(_landSound, transform.GetChild(0).transform.position);
    }
}