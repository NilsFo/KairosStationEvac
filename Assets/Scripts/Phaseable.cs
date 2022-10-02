using System;
using UnityEngine;

public abstract class Phaseable : MonoBehaviour
{
    protected GameState Game;
    public abstract void Reset();
    public abstract void PhaseEvacuate();
    public abstract void PhasePlanning();

    public virtual void PhaseExplosion()
    {
    }

    public virtual void OnWin()
    {
    }

    public virtual void OnWinOnce()
    {
    }

    public virtual void Start()
    {
        Game = FindObjectOfType<GameState>();
        Game.SubscribeToPhases(this);
    }

    public virtual void OnEnable()
    {
        if (Game != null)
        {
            Game.SubscribeToPhases(this);
        }
    }

    public virtual void OnDisable()
    {
        if (Game != null)
        {
            Game.UnsubscribeFromPhases(this);
        }
    }
}