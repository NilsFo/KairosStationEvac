using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CrewmateExit : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int Full = Animator.StringToHash("full");

    public void RescueCrewmate(GameObject caller)
    {
        CrewmateController controller = caller.GetComponent<CrewmateController>();
        if (controller == null) return;
        controller.Rescue();
        animator.SetBool(Full, true);
    }
}