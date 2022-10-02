using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserSegment : Phaseable
{
    public LaserEmitter myParentEmitter;
    private List<LaserTargetable> myTargets = new List<LaserTargetable>();

    public override void Start()
    {
        base.Start();
        Reset();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        LaserTargetable targetable = other.gameObject.GetComponent<LaserTargetable>();
        if (targetable != null)
        {
            myTargets.Add(targetable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LaserTargetable targetable = other.gameObject.GetComponent<LaserTargetable>();
        if (targetable != null)
        {
            myTargets.Remove(targetable);
            targetable.ResetTimer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        List<LaserTargetable> killedList = new List<LaserTargetable>();
        foreach (LaserTargetable t in myTargets)
        {
            if (!t.gameObject != myParentEmitter.gameObject)
            {
                t.Frizzle(Time.deltaTime);
                if (t.Broken)
                {
                    killedList.Add(t);
                }
            }
        }

        foreach (LaserTargetable killed in killedList)
        {
            myTargets.Remove(killed);
        }
    }

    public override void Reset()
    {
        myTargets.Clear();
    }

    public override void PhaseEvacuate()
    {
    }

    public override void PhasePlanning()
    {
    }
}