using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject offGameObject;
    [SerializeField] private GameObject onGameObject;

    [SerializeField] private bool InitState = false;
    
    [SerializeField] private bool currentState;

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        currentState = InitState;
        UpdateState();
    }

    public void ToggleState()
    {
        currentState = !currentState;
        UpdateState();
    }

    public void SetStateOn()
    {
        currentState = true;
        UpdateState();
    }
    
    public void SetStateOff()
    {
        currentState = false;
        UpdateState();
    }

    private void UpdateState()
    {
        if (currentState)
        {
            onGameObject.SetActive(true);
            offGameObject.SetActive(false);
        }
        else
        {
            onGameObject.SetActive(false);
            offGameObject.SetActive(true);
        }
    }
}
