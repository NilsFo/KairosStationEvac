using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewmateExit : MonoBehaviour
{
    public void RescueCrewmate(GameObject caller)
    {
        CrewmateController controller = caller.GetComponent<CrewmateController>();
        if (controller == null) return;
        controller.Rescue();
        GetComponent<AudioSource>().Play();
    }
}