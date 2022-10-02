using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    private List<Phaseable> myObservers = new List<Phaseable>();
    private List<CrewmateController> allCrewmates = new List<CrewmateController>();
    public CrewmateController selectedCrewmate;
    public int CrewmateCount => allCrewmates.Count;

    public GameObject floatingTextPrefab;

    private void OnEnable()
    {
        var crewmates = FindObjectsOfType<CrewmateController>();
        foreach (CrewmateController c in crewmates)
        {
            allCrewmates.Add(c);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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

    public int GetRescuedCrewmateCount()
    {
        int rescueCount = 0;
        foreach (CrewmateController crewmateController in allCrewmates)
        {
            if (crewmateController.rescued)
            {
                rescueCount++;
            }
        }

        return rescueCount;
    }

    public void CheckWinCondition()
    {
        int rescueCount = GetRescuedCrewmateCount();
        if (rescueCount == CrewmateCount)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        selectedCrewmate = null;
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
                OnPhasePlanning();
                break;
            case Phase.EvacuationPhase:
                OnPhaseEvacuate();
                break;
            case Phase.WinState:
                OnPhaseWin();
                break;
        }
    }

    private void OnPhaseEvacuate()
    {
        onPhasePlaying.Invoke();

        // Setting player control
        foreach (CrewmateController c in allCrewmates)
        {
            c.playerControlled = false;
        }

        selectedCrewmate.playerControlled = true;

        foreach (var p in myObservers)
        {
            p.PhaseEvacuate();
        }

        if (selectedCrewmate == null)
        {
            Debug.LogError("BIG ERROR! NO CREWMATE SELECTED!!!!");
            currentPhase = Phase.Unknown;
            return;
        }
    }

    private void OnPhasePlanning()
    {
        ResetAndCleanUp();
        onPhasePlanning.Invoke();
        foreach (var p in myObservers)
        {
            p.PhasePlanning();
        }
    }

    private void OnPhaseWin()
    {
        onWin.Invoke();
        foreach (var p in myObservers)
        {
            p.PhaseWin();
        }
    }

    private void ResetAndCleanUp()
    {
        onResetGameplay.Invoke();
        selectedCrewmate = null;

        foreach (var p in myObservers)
        {
            p.Reset();
        }
    }

    public void BackToMainMenu()
    {
        print("Back to main menu.");
        // TODO implement
    }

    public void DisplayFloatingText(Vector3 position, string text, float duration=3f, float fontSize = 0.69f,
        float velocity_Y = 0.15f, float velocity_Z = 0f, float velocity_X = 0f)
    {
        var newObj = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        FloatingText flt = newObj.GetComponent<FloatingText>();
        flt.text = text;
        flt.duration = duration;
        flt.velocity.y = velocity_Y;
        flt.velocity.x = velocity_X;
        flt.velocity.z = velocity_Z;
        flt.fontSize = fontSize;
    }
}