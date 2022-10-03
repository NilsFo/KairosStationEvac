using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UITilemapObjective : Phaseable
{
    public GameObject winPopup;
    public Tilemap myMap;
    public Vector2Int firstDigit;
    public Vector2Int secondDigit;

    public GameObject tilemapPlanning;
    public GameObject tilemapEvac;

    public UITilemapDictionary dictionary;
    public int timeRemaining;
    private GamePhaseLoop _phaseLoop;

    // Start is called before the first frame update
    public override void Reset()
    {
        _phaseLoop = FindObjectOfType<GamePhaseLoop>();
        UpdateText();
    }

    public override void PhaseEvacuate()
    {
        UpdateText();
    }

    public override void PhasePlanning()
    {
        UpdateText();
    }

    public override void Start()
    {
        base.Start();
        _phaseLoop = FindObjectOfType<GamePhaseLoop>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining = _phaseLoop.GetTimerForUI();
        UpdateText();
    }

    private void UpdateText()
    {
        winPopup.SetActive(Game.levelWon);
        tilemapPlanning.SetActive(false);
        tilemapEvac.SetActive(false);
        timeRemaining = _phaseLoop.GetTimerForUI();

        if (!Game.levelWon)
        {
            if (Game.currentPhase == GameState.Phase.PlanningPhase)
            {
                tilemapPlanning.SetActive(true);
            }

            if (Game.currentPhase == GameState.Phase.EvacuationPhase)
            {
                tilemapEvac.SetActive(true);
                Vector3Int f = new Vector3Int(firstDigit.x, firstDigit.y, 0);
                Vector3Int s = new Vector3Int(secondDigit.x, secondDigit.y, 0);

                int digit = timeRemaining;
                if (timeRemaining >= 10)
                {
                    myMap.SetTile(f, dictionary.Get(1));
                    digit = digit - 10;
                }
                else
                {
                    myMap.SetTile(f, dictionary.Get(0));
                }

                myMap.SetTile(s, dictionary.Get(digit));
            }
        }
    }
}