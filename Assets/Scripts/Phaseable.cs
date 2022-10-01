using System;
using UnityEngine;
public abstract class Phaseable: MonoBehaviour {
    
    protected GameState Game;
    public abstract void Reset();
    public abstract void PhaseEvacuate();
    public abstract void PhasePlanning();
    public void PhaseWin() {}
    
    public virtual void Start() {
        Game = FindObjectOfType<GameState>();
    }
}
