using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum OnOffType
{
    Button,
    Toggle,
    Switch,
}

public class OnOffBehaviourScript : Phaseable
{
    [Header("HookUps")]
    [SerializeField] private GameObject offGameObject;
    [SerializeField] private GameObject onGameObject;
    [SerializeField] private GameObject disableGameObject;
    [SerializeField] private List<GameObject> altOnGameObject;
    
    [Header("Button Settings")]
    [SerializeField] private OnOffType ButtonType = OnOffType.Toggle;
    
    [Header("Init && Reset Field-Values")]
    [SerializeField] private bool InitActivationState = false;
    [SerializeField] private bool InitPowerState = true;
    [SerializeField] private float InitTimer = 4f;

    [Header("Runtime Field-Values")]
    [Tooltip("Only debug purpose!")]
    [SerializeField] private bool currentActivationState;
    [SerializeField] private bool currentPowerState;
    [SerializeField] private float currentTimer = 0f;
    
    [Header("Events")]
    public UnityEvent<GameObject> OmEnableEvent;
    public UnityEvent<GameObject> OmDisableEvent;
    
    public UnityEvent<GameObject> OmActivateEvent;
    public UnityEvent<GameObject> OmDeactivateEvent;

    [Header("Flavortext")] 
    public string OnActivationText = "";
    public string OnDeactivationText = "";
    public string OnEnablePowerText = "";
    public string OnDisablePowerText = "";

    [Header("Flavortext Denied Activation")]
    public string OnNoPower = "";
    public string OnAlreadyUsed = "";

    public bool IsToggleable => (ButtonType == OnOffType.Toggle);

    public bool IsSwitch => (ButtonType == OnOffType.Switch);

    public override void Start()
    {
        base.Start();
        OmEnableEvent ??= new UnityEvent<GameObject>();
        OmDisableEvent ??= new UnityEvent<GameObject>();
        OmActivateEvent ??= new UnityEvent<GameObject>();
        OmDeactivateEvent ??= new UnityEvent<GameObject>();

        ResetState();
    }

    private void Update()
    {
        if (IsSwitch && currentPowerState)
        {
            if (currentTimer > 0)
            {
                currentTimer -= Time.deltaTime;
                UpdateState();
            } else if (currentTimer <= 0)
            {
                SetStateOff(gameObject);
            }
        }
    }

    public void ResetState()
    {
        if (InitPowerState)
        {
            EnabelInteraktion(gameObject);
        }
        else
        {
            DisableInteraktion(gameObject);
        }
        
        if (InitActivationState)
        {
            SetStateOn(gameObject);
        }
        else
        {
            SetStateOff(gameObject);
        }
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
        if(!currentPowerState) return;
        if(!IsToggleable&& currentActivationState != InitActivationState) return;
        if (IsToggleable)
        {
            if (currentActivationState)
            {
                SetStateOff(caller);
            }
            else
            {
                SetStateOn(caller);
            }
        }
        else
        {
            if (InitActivationState)
            {
                SetStateOff(caller);
            }
            else
            {
                SetStateOn(caller);
            }
        }
    }

    public void SetStateOn(GameObject caller)
    {
        if(!currentPowerState) return;
        if(currentActivationState) return;
        if(!IsToggleable && currentActivationState != InitActivationState) return;
        currentActivationState = true;
        OmActivateEvent.Invoke(caller);
        if (IsSwitch) currentTimer = InitTimer;
        Debug.Log(IsSwitch);
        UpdateState();
    }
    
    public void SetStateOff(GameObject caller)
    {
        if(!currentPowerState) return;
        if(!currentActivationState) return;
        if(!IsToggleable && currentActivationState != InitActivationState) return;
        currentActivationState = false;
        OmDeactivateEvent.Invoke(caller);
        UpdateState();
    }

    public void EnabelInteraktion(GameObject caller)
    {
        if (currentPowerState) return;
        currentPowerState = true;
        OmEnableEvent.Invoke(caller);
        if(currentActivationState) OmActivateEvent.Invoke(caller);
        UpdateState();
    }
    
    public void DisableInteraktion(GameObject caller)
    {
        if (!currentPowerState) return;
        currentPowerState = false;
        OmDisableEvent.Invoke(caller);
        if(currentActivationState) OmDeactivateEvent.Invoke(caller);
        UpdateState();
    }

    private void UpdateState()
    {
        if (!currentPowerState)
        {
            disableGameObject.SetActive(true);
            DeactiveOnObjects();
            offGameObject.SetActive(false);
        }
        else
        {
            if (currentActivationState)
            {
                disableGameObject.SetActive(false);
                ActiveOnObjects();
                offGameObject.SetActive(false);
            }
            else
            {
                disableGameObject.SetActive(false);
                DeactiveOnObjects();
                offGameObject.SetActive(true);
            }
        }
    }

    private void ActiveOnObjects()
    {
        if (IsSwitch)
        {
            onGameObject.SetActive(false);
            var parts = InitTimer / altOnGameObject.Count; //5
            int chosenIndex = (int) (currentTimer / parts); // 4,3,2,1,0

            for (int i = 0; i < altOnGameObject.Count; i++)
            {
                var obj = altOnGameObject[i];
                if(i == chosenIndex)
                {
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }
        else
        {
            DeactiveOnObjects();
            onGameObject.SetActive(true);
        }
    }
    
    private void DeactiveOnObjects()
    {
        onGameObject.SetActive(false);
        for (int i = 0; i < altOnGameObject.Count; i++)
        {
            var obj = altOnGameObject[i];
            obj.SetActive(false);
        }
    }
}
