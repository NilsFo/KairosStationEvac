using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public enum State
    {
        Unknown,
        EvacuationPhase,
        PlanningPhase,
        WinState
    }

    private bool firstCleanDone = false;
    public State currentState = State.Unknown;
    private State _lastKnownState;
    public UnityEvent onStatePlaying;
    public UnityEvent onStatePlanning;
    public UnityEvent onStateWin;
    public UnityEvent onResetGameplay;

    // Start is called before the first frame update
    void Start()
    {
        if (onStatePlaying == null) onStatePlaying = new UnityEvent();
        if (onStatePlanning == null) onStatePlanning = new UnityEvent();
        if (onStateWin == null) onStateWin = new UnityEvent();
        if (onResetGameplay == null) onResetGameplay = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstCleanDone)
        {
            firstCleanDone = true;
            ResetAndCleanUp();
        }

        if (_lastKnownState != currentState)
        {
            OnStateChanged();
            _lastKnownState = currentState;
        }
    }

    public void WinGame()
    {
        print("A winner is you!");
        currentState = State.WinState;
    }

    private void OnStateChanged()
    {
        print("The state has changed. From: '" + _lastKnownState + "' to '" + currentState);
        //TODO callback to other elements??

        switch (currentState)
        {
            case State.PlanningPhase:
                ResetAndCleanUp();
                onResetGameplay.Invoke();
                break;
            case State.EvacuationPhase:
                onStatePlaying.Invoke();
                break;
            case State.WinState:
                onStateWin.Invoke();
                break;
        }
    }

    private void ResetAndCleanUp()
    {
        onResetGameplay.Invoke();
        // TODO Implement
        // When this is called, reset all objects for the next loop
    }
}