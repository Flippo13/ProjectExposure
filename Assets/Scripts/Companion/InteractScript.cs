using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractScript : MonoBehaviour {

    public VacuumArea vacuumArea;
    public LayerMask suckableLayer; 
    public float suckSpeed;
    private float suckTime;

    private int _trashCount;

	// Use this for initialization
	void Awake () {
        _trashCount = 0;
	}

    public void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == suckableLayer) {
            _trashCount++;
            vacuumArea.RemoveTransfromFromList(other.transform);
            Destroy(other.gameObject);
        }
    }

    public void Suck()
    {
        Debug.Log("Suck");
        suckTime += 0.25f * Time.deltaTime;
        if(suckTime > 1)
        {
            suckTime = 0; 
        }
        if(suckTime < 0)
        {
            suckTime = 1; 
        }

        for (int i = 0; i < vacuumArea.suckableObjectsList.Count; i++)
        {
            Vector3 suckDir = ( transform.position - vacuumArea.suckableObjectsList[i].position ).normalized;
            vacuumArea.suckableObjectsList[i].Translate(suckDir * 0.1f);
        }
    }

    public void SetSuckTime(float time) {
        suckTime = time;
    }

    public int GetTrashCount() {
        return _trashCount;
    }
}
