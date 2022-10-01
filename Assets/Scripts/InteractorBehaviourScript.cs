using System.Collections.Generic;
using UnityEngine;

public class InteractorBehaviourScript : MonoBehaviour
{
    private List<InteractionBehaviourScript> interactions;

    private void OnEnable()
    {
        interactions = new List<InteractionBehaviourScript>();
    }

    public void AddInteraction(InteractionBehaviourScript interactionBehaviourScript)
    {
        interactions.Add(interactionBehaviourScript);
    }
    
    public void RemoveInteraction(InteractionBehaviourScript interactionBehaviourScript)
    {
        interactions.Remove(interactionBehaviourScript);
    }

    public void TriggerInteractions()
    {
        for (int i = 0; i < interactions.Count; i++)
        {
            interactions[i].TriggerInteraction();
        }
    }
}
