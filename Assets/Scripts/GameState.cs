using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public enum Phase
    {
        Unknown,
        EvacuationPhase,
        PlanningPhase,
        WinState
    }

    private bool firstCleanDone = false;
    public Phase currentPhase = Phase.Unknown;
    private Phase _lastKnownPhase;
    public UnityEvent onPhasePlaying;
    public UnityEvent onPhasePlanning;
    public UnityEvent onWin;
    public UnityEvent onResetGameplay;

    private List<Phaseable> myObservers;

    // Start is called before the first frame update
    void Start()
    {
        myObservers = new List<Phaseable>();
        if (onPhasePlaying == null) onPhasePlaying = new UnityEvent();
        if (onPhasePlanning == null) onPhasePlanning = new UnityEvent();
        if (onWin == null) onWin = new UnityEvent();
        if (onResetGameplay == null) onResetGameplay = new UnityEvent();
    }

    public void SubscribeToPhases(Phaseable p)
    {
        if (!myObservers.Contains(p))
        {
            myObservers.Add(p);
        }
    }

    public void UnsubscribeFromPhases(Phaseable p)
    {
        if (myObservers.Contains(p))
        {
            myObservers.Remove(p);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!firstCleanDone)
        {
            firstCleanDone = true;
            ResetAndCleanUp();
        }

        if (_lastKnownPhase != currentPhase)
        {
            OnStateChanged();
            _lastKnownPhase = currentPhase;
        }
    }

    public void WinGame()
    {
        print("A winner is you!");
        currentPhase = Phase.WinState;
    }

    private void OnStateChanged()
    {
        print("The state has changed. From: '" + _lastKnownPhase + "' to '" + currentPhase);
        //TODO callback to other elements??

        switch (currentPhase)
        {
            case Phase.PlanningPhase:
                ResetAndCleanUp();
                onPhasePlanning.Invoke();
                foreach (var p in myObservers)
                {
                    p.PhasePlanning();
                }

                break;
            case Phase.EvacuationPhase:
                onPhasePlaying.Invoke();
                foreach (var p in myObservers)
                {
                    p.PhaseEvacuate();
                }

                break;
            case Phase.WinState:
                onWin.Invoke();
                foreach (var p in myObservers)
                {
                    p.PhaseWin();
                }

                break;
        }
    }

    private void ResetAndCleanUp()
    {
        onResetGameplay.Invoke();

        foreach (var p in myObservers)
        {
            p.Reset();
        }
    }
}