using UnityEngine;
using UnityEngine.Events;

public class InteractionBehaviourScript : MonoBehaviour
{
    public UnityEvent triggerEvent;

    private void Start()
    {
        triggerEvent ??= new UnityEvent();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var interactor = col.gameObject.GetComponent<InteractorBehaviourScript>();
        Debug.Log("Enter", this);
        if(interactor == null) return;

        interactor.AddInteraction(this);
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        var interactor = col.gameObject.GetComponent<InteractorBehaviourScript>();
        if(interactor == null) return;

        interactor.RemoveInteraction(this);
    }

    public void TriggerInteraction()
    {
        triggerEvent.Invoke();
    }
}
