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
    public UnityEvent<GameObject> OmEnableEvent;
    public UnityEvent<GameObject> OmDisableEvent;
    
    public UnityEvent<GameObject> OmActivateEvent;
    public UnityEvent<GameObject> OmDeactivateEvent;
    
    // Crewmate rescue
    private CrewmateExit _crewmateExit;

    public override void Start()
    {
        base.Start();
        _crewmateExit = GetComponent<CrewmateExit>();
        OmEnableEvent ??= new UnityEvent<GameObject>();
        OmDisableEvent ??= new UnityEvent<GameObject>();
        OmActivateEvent ??= new UnityEvent<GameObject>();
        OmDeactivateEvent ??= new UnityEvent<GameObject>();
        
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
    public void ToggleState(GameObject caller)
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
            OmActivateEvent.Invoke(caller);
        }
        else
        {
            OmDeactivateEvent.Invoke(caller);
        }
        UpdateState();
    }

    public void SetStateOn(GameObject caller)
    {
        if(!currentActiveEnable) return;
        if(!IsToggleable && currentState != InitState) return;
        currentState = true;
        OmActivateEvent.Invoke(caller);
        UpdateState();
    }
    
    public void SetStateOff(GameObject caller)
    {
        if(!currentActiveEnable) return;
        if(!IsToggleable && currentState != InitState) return;
        currentState = false;
        OmDeactivateEvent.Invoke(caller);
        UpdateState();
    }

    public void EnabelInteraktion(GameObject caller)
    {
        if (currentActiveEnable) return;
        currentActiveEnable = true;
        OmEnableEvent.Invoke(caller);
        UpdateState();
    }
    
    public void DisableInteraktion(GameObject caller)
    {
        if (!currentActiveEnable) return;
        currentActiveEnable = false;
        OmDisableEvent.Invoke(caller);
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
