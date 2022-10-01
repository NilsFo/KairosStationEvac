using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhaseLoop : MonoBehaviour
{
    public GameState myGameState;
    public int phaseLength = 10;
    private float timer;
    private GameState.Phase _currentPhase = GameState.Phase.Unknown;

    // Start is called before the first frame update
    void Start()
    {
        myGameState.currentPhase = GameState.Phase.PlanningPhase;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _currentPhase = myGameState.currentPhase;

        if (_currentPhase == GameState.Phase.Unknown)
        {
            Debug.LogError("UNKNOWN SATE!");
        }

        if (_currentPhase == GameState.Phase.WinState || _currentPhase == GameState.Phase.Unknown)
        {
            return;
        }

        if (_currentPhase == GameState.Phase.EvacuationPhase)
        {
            timer += Time.deltaTime;
            if (timer >= phaseLength)
            {
                timer = 0;
                NextPhase();
            }
        }
    }

    public void NextPhase()
    {
        timer = 0;
        if (_currentPhase == GameState.Phase.PlanningPhase)
        {
            myGameState.currentPhase = GameState.Phase.EvacuationPhase;
        }
        else
        {
            myGameState.currentPhase = GameState.Phase.PlanningPhase;
        }
    }

    public string GetTimerFormated()
    {
        if (_currentPhase == GameState.Phase.EvacuationPhase)
        {
            int i = (int) timer;
            return (phaseLength - i).ToString();
        }

        return "N/A";
    }
    
}