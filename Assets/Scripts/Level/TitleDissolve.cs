using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDissolve : MonoBehaviour {


    [SerializeField]
    private GameObject _title;

    [SerializeField]
    private GameObject _titleParticles;


    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            _title.GetComponent<Animator>().Play("Titledissolve");
            _titleParticles.GetComponent<Animator>().Play("Titledissolveparticles");
            Debug.Log("Player exited");
        }

       
    }
}
