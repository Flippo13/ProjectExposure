using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineManager : MonoBehaviour {

    public TurbineSetUp[] turbine;
   
    private void Start()
    {
        SetUpTurbine();

    }

    private void SetUpTurbine()
    {
        for (int i = 0; i < turbine.Length; i++)
        {
            if (turbine[i].turbine != null && turbine[i].console != null && turbine[i].powerGrid != null && turbine[i].trashArea != null && !turbine[i].isBeingDropped)
            {
                turbine[i].powerGrid.cableConntectedEvent.AddListener(turbine[i].turbine.CableConnected);
                turbine[i].console.turbineButtonEvent.AddListener(turbine[i].turbine.Activate);
                turbine[i].trashArea.trashCleanedEvent.AddListener(turbine[i].turbine.TrashCleaned);
            }
            else
            {
                Debug.Log("You forgot to assign someting in " + turbine[i]);
                Debug.Log("Is turbine null? " + (turbine[i].turbine == null));
                Debug.Log("Is turbine console null? " + (turbine[i].console == null));
                Debug.Log("Is turbine powerGrid null? " + (turbine[i].powerGrid == null));
                Debug.Log("Is turbine trashArea null? " + (turbine[i].trashArea == null));
            }

            if (turbine[i].isBeingDropped && turbine[i].turbineLandingZone != null)
            {
                turbine[i].turbineLandingZone.Console = turbine[i].console;
                turbine[i].turbineLandingZone.PowerGrid = turbine[i].powerGrid; 
            }
        }
    }
}
