using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILogic : MonoBehaviour
{
    public TMP_Text gameStateText;
    private GameState _gameState;
    private GamePhaseLoop _phaseLoop;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameState>();
        _phaseLoop = FindObjectOfType<GamePhaseLoop>();
    }

    // Update is called once per frame
    void Update()
    {
        gameStateText.text = "State: " + _gameState.currentPhase + " [" + _phaseLoop.GetTimerFormatted() +
                             "]. Rescued: " + _gameState.GetRescuedCrewmateCount() + "/" + _gameState.CrewmateCount;
    }
}