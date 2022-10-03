using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCMainMenuTurnAround : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject o = other.gameObject;
        NPCMainMenuMovement mainMenuMovement = o.GetComponent<NPCMainMenuMovement>();
        if (mainMenuMovement != null)
        {
            mainMenuMovement.TurnAround();
        }
    }
}
