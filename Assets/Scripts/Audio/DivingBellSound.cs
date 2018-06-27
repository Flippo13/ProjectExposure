using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingBellSound : MonoBehaviour {



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

    public void DoorSound(string open)
    {
        if (open == "true")
        {
            FMODUnity.RuntimeManager.PlayOneShot(_doorOpen, transform.position);
        }
        else
            FMODUnity.RuntimeManager.PlayOneShot(_doorClose, transform.position);

    }


    public void MoveSound()
    {

        _moveSound = FMODUnity.RuntimeManager.CreateInstance(_divingBellMove);
        _moveSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        _moveSound.start();

    }

    public void LandSound()
    {
        _moveSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMODUnity.RuntimeManager.PlayOneShot(_landSound, transform.position);
    }
}
