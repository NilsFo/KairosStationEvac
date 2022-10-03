using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhaseLoop : MonoBehaviour
{
    public GameState myGameState;
    public int phaseLengthEvac = 10;
    public int phaseLengthExplosion = 3;
    private float timer;
    private GameState.Phase _currentPhase = GameState.Phase.Unknown;

    public float explosionPhasePhaseShakeMagnitude = 2f;
    public float explosionPhaseShakeDuration = 5f;

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
            Debug.LogError("UNKNOWN STATE!");
        }

        if (_currentPhase == GameState.Phase.PlanningPhase && myGameState.levelWon)
        {
            SetPhaseEvac();
            return;
        }

        //////////////////////////////////////
        //Checking for inputs
        //////////////////////////////////////

        /// Back to menu / cancel
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (myGameState.levelWon)
            {
                myGameState.NextLevel();
                return;
            }

            if (_currentPhase == GameState.Phase.EvacuationPhase)
            {
                SetPhasePlanning();
            }

            myGameState.BackToMainMenu();
            return;
        }

        // Changing phase by pressing space or Q
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.Space))
        {
            if (myGameState.levelWon)
            {
                myGameState.NextLevel();
                return;
            }

            switch (_currentPhase)
            {
                case GameState.Phase.EvacuationPhase:
                    SetPhasePlanning();
                    return;
                case GameState.Phase.ExplosionPhase:
                    SetPhasePlanning();
                    return;
                case GameState.Phase.PlanningPhase:
                    myGameState.selectedCrewmate = null;
                    SetPhaseEvac();
                    return;
            }
        }

        if (_currentPhase == GameState.Phase.Unknown)
        {
            return;
        }


        if (_currentPhase == GameState.Phase.ExplosionPhase)
        {
            timer += Time.deltaTime;
            if (timer >= phaseLengthExplosion)
            {
                timer = 0;
                SetPhasePlanning();
            }
        }
    }

    private void FixedUpdate() {
        if (_currentPhase == GameState.Phase.EvacuationPhase)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= phaseLengthEvac)
            {
                timer = 0;
                if (myGameState.levelWon)
                {
                    SetPhasePlanning();
                }
                else
                {
                    SetPhaseExplosion();
                }
            }
        }
    }

    public void SetPhasePlanning()
    {
        timer = 0;
        myGameState.currentPhase = GameState.Phase.PlanningPhase;
    }

    public void SetPhaseEvac()
    {
        timer = 0;
        myGameState.currentPhase = GameState.Phase.EvacuationPhase;
    }

    public void SetPhaseExplosion()
    {
        timer = 0;
        myGameState.currentPhase = GameState.Phase.ExplosionPhase;
        myGameState.ShakeCamera(explosionPhasePhaseShakeMagnitude, explosionPhaseShakeDuration);
    }

    public string GetTimerFormatted()
    {
        if (_currentPhase == GameState.Phase.EvacuationPhase)
        {
            int i = (int) timer;
            return (phaseLengthEvac - i).ToString();
        }

        return "N/A";
    }

    public int GetTimerForUI()
    {
        int i = (int) timer;
        return phaseLengthEvac - i;
    }
}