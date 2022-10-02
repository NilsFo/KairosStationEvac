using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserTargetable : Phaseable
{
    public float breakTime = 1.337f;
    private float _breakTimeCurrent = 0;
    public bool Broken => _breakTimeCurrent >= breakTime;
    public UnityEvent brokenByLaser;

    // Start is called before the first frame update
    public override void Reset()
    {
        ResetTimer();
    }

    public override void PhaseEvacuate()
    {
    }

    public override void PhasePlanning()
    {
    }

    public override void Start()
    {
        base.Start();
        if (brokenByLaser == null) brokenByLaser = new UnityEvent();
        Reset();
    }

    public void Frizzle(float amount)
    {
        if (Broken)
        {
            Debug.Log("YOU FIZZLE SOMETHING THAT IS ALREADY BROKEN! DONT DO THAT!");
            return;
        }

        _breakTimeCurrent += amount;
        print("fizzle: " + _breakTimeCurrent + "/" + breakTime);

        if (Broken)
        {
            Break();
        }
    }

    public void ResetTimer()
    {
        _breakTimeCurrent = 0;
    }

    public void Break()
    {
        brokenByLaser.Invoke();
    }
}