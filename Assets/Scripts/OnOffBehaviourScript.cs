using UnityEngine;
using UnityEngine.Events;

public class OnOffBehaviourScript : Phaseable
{
    [Header("HookUps")]
    [SerializeField] private GameObject offGameObject;
    [SerializeField] private GameObject onGameObject;
    [SerializeField] private GameObject disableGameObject;

    [Header("Init && Reset Field-Values")]
    [SerializeField] private bool IsToggleable = true;
    [SerializeField] private bool InitState = false;
    [SerializeField] private bool InitActiveEnable = true;
    
    [Header("Runtime Field-Values")]
    [Tooltip("Only debug purpose!")]
    [SerializeField] private bool currentState;
    [SerializeField] private bool currentActiveEnable;
    
    [Header("Events")]
    public UnityEvent OmEnableEvent;
    public UnityEvent OmDisableEvent;
    
    public UnityEvent OmActivateEvent;
    public UnityEvent OmDeactivateEvent;

    public override void Start()
    {
        base.Start();
        OmEnableEvent ??= new UnityEvent();
        OmDisableEvent ??= new UnityEvent();
        OmActivateEvent ??= new UnityEvent();
        OmDeactivateEvent ??= new UnityEvent();
        
        ResetState();
    }

    public void ResetState()
    {
        currentState = InitState;
        currentActiveEnable = InitActiveEnable;
        UpdateState();
    }

    public override void Reset()
    {
        ResetState();
    }
    public override void PhaseEvacuate() {
        ResetState();
    }
    public override void PhasePlanning() {
        ResetState();
    }
    public void ToggleState()
    {
        if(!currentActiveEnable) return;
        if(!IsToggleable && currentState != InitState) return;
        if (IsToggleable)
        {
            currentState = !currentState;
        }
        else
        {
            currentState = !InitState;
        }
        if (currentState)
        {
            OmActivateEvent.Invoke();
        }
        else
        {
            OmDeactivateEvent.Invoke();
        }
        UpdateState();
    }

    public void SetStateOn()
    {
        if(!currentActiveEnable) return;
        if(!IsToggleable && currentState != InitState) return;
        currentState = true;
        OmActivateEvent.Invoke();
        UpdateState();
    }
    
    public void SetStateOff()
    {
        if(!currentActiveEnable) return;
        if(!IsToggleable && currentState != InitState) return;
        currentState = false;
        OmDeactivateEvent.Invoke();
        UpdateState();
    }

    public void EnabelInteraktion()
    {
        if (currentActiveEnable) return;
        currentActiveEnable = true;
        OmEnableEvent.Invoke();
        UpdateState();
    }
    
    public void DisableInteraktion()
    {
        if (!currentActiveEnable) return;
        currentActiveEnable = false;
        OmDisableEvent.Invoke();
        UpdateState();
    }

    private void UpdateState()
    {
        if (!currentActiveEnable)
        {
            disableGameObject.SetActive(true);
            onGameObject.SetActive(false);
            offGameObject.SetActive(false);
        }
        else
        {
            if (currentState)
            {
                disableGameObject.SetActive(false);
                onGameObject.SetActive(true);
                offGameObject.SetActive(false);
            }
            else
            {
                disableGameObject.SetActive(false);
                onGameObject.SetActive(false);
                offGameObject.SetActive(true);
            }
        }
    }
}
