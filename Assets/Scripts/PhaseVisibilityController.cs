using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseVisibilityController : Phaseable
{
    public GameObject myObject;
    public bool visibleInEvacuate = false;
    public bool visibleInPlanning = false;
    public bool visibleInExplosion = false;
    public bool visibleInTutorial = false;

    // Start is called before the first frame update
    public override void Reset()
    {
        UpdateVisibility();
    }

    public override void PhaseEvacuate()
    {
        UpdateVisibility();
    }

    public override void PhasePlanning()
    {
        UpdateVisibility();
    }

    public override void Start()
    {
        base.Start();
        UpdateVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        myObject.SetActive(false);
        switch (Game.currentPhase)
        {
            case GameState.Phase.Tutorial:
                myObject.SetActive(visibleInTutorial);
                break;
            case GameState.Phase.EvacuationPhase:
                myObject.SetActive(visibleInEvacuate);
                break;
            case GameState.Phase.ExplosionPhase:
                myObject.SetActive(visibleInExplosion);
                break;
            case GameState.Phase.PlanningPhase:
                myObject.SetActive(visibleInPlanning);
                break;
        }
    }
}