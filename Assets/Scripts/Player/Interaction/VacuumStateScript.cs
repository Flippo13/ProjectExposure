using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//needed for the animation functions
public class VacuumStateScript : MonoBehaviour {

    public VacuumScript vacuum;

    //setting hand position by animation
    public void SetVacuumToHand() {
        if (vacuum.GetVacuumState() == VacuumState.Player || vacuum.GetVacuumState() == VacuumState.Free) return; //do nothing if the player already has the vacuum gun or if it is lying around

        vacuum.SetVacuumState(VacuumState.CompanionHand);
    }

    public void SetVacuumToBack() {
        if (vacuum.GetVacuumState() == VacuumState.Player || vacuum.GetVacuumState() == VacuumState.Free) return; //do nothing if the player already has the vacuum gun or if it is lying around

        vacuum.SetVacuumState(VacuumState.CompanionBack);
    }
}
