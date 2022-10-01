using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewmateExit : MonoBehaviour
{

    public void RescueCrewmate(CrewmateController crewmateController)
    {
        crewmateController.Rescue();
    }

}
