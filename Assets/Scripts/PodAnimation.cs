using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int Full = Animator.StringToHash("full");

    public void Fill()
    {
        animator.SetBool(Full,true);
    }

    public void Empty()
    {
        animator.SetBool(Full,false);
    }
}
