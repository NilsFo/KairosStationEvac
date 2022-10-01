using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBehaviourScript : MonoBehaviour
{

    [SerializeField] private string triggerName;
    [SerializeField] private bool isToggle = false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        var interactor = col.gameObject.GetComponent<InteractorBehaviourScript>();
        if(interactor == null) return;

        interactor.AddInteraction(this);
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        var interactor = col.gameObject.GetComponent<InteractorBehaviourScript>();
        if(interactor == null) return;

        interactor.RemoveInteraction(this);
    }

    public void TriggerInteraction()
    {
        Debug.Log("Triggert:" + triggerName + " toggle " + isToggle);
    }
}
