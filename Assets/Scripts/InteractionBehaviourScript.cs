using UnityEngine;
using UnityEngine.Events;

public class InteractionBehaviourScript : MonoBehaviour
{
    public UnityEvent<GameObject> triggerEvent;

    private void Start()
    {
        triggerEvent ??= new UnityEvent<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var interactor = col.gameObject.GetComponent<InteractorBehaviourScript>();
        if (interactor == null) return;

        interactor.AddInteraction(this);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        var interactor = col.gameObject.GetComponent<InteractorBehaviourScript>();
        if (interactor == null) return;

        interactor.RemoveInteraction(this);
    }

    public void TriggerInteraction(GameObject currentGameObject)
    {
        triggerEvent.Invoke(currentGameObject);
    }
}