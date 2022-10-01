using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject offGameObject;
    [SerializeField] private GameObject onGameObject;

    [SerializeField] private bool InitState = false;
    
    private bool currentState;

    public void ResetState()
    {
        currentState = InitState;
        UpdateState();
    }

    public void ToggleState()
    {
        
    }

    public void SetStateOn()
    {
        currentState = true;
    }
    
    public void SetStateOff()
    {
        currentState = false;
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
