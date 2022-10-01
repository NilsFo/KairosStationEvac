using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhaseLoop : MonoBehaviour
{
    public GameState myGameState;
    public int phaseLength = 10;
    private float timer;
    private GameState.State currentState = GameState.State.Unknown;

    // Start is called before the first frame update
    void Start()
    {
        myGameState.currentState = GameState.State.PlanningPhase;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        currentState = myGameState.currentState;

        if (currentState == GameState.State.WinState || currentState == GameState.State.Unknown)
        {
            return;
        }

        if (timer >= phaseLength)
        {
            timer = 0;
            NextPhase();
        }
    }

    public void NextPhase()
    {
        if (currentState == GameState.State.PlanningPhase)
        {
            myGameState.currentState = GameState.State.EvacuationPhase;
        }
        else
        {
            myGameState.currentState = GameState.State.PlanningPhase;
        }
    }
}