using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnableWall : Phaseable
{
    public List<GameObject> burningFrames;
    public List<float> framePercentages;

    public LaserTargetable myLaserTarget;
    public BoxCollider2D myCollider;

    public float brokenPercent;
    public bool broken;

    private void Awake()
    {
        Debug.Assert(burningFrames.Count == framePercentages.Count);
    }

    // Start is called before the first frame update
    public override void Reset()
    {
        myLaserTarget.ResetTimer();
        myLaserTarget.Reset();
        UpdateState();
    }

    public override void PhaseEvacuate()
    {
    }

    public override void PhasePlanning()
    {
    }

    public override void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        brokenPercent = myLaserTarget.GetDestructionPercent();
        broken = myLaserTarget.Broken;

        foreach (var o in burningFrames)
        {
            o.SetActive(false);
        }

        for (int i = 0; i < framePercentages.Count; i++)
        {
            GameObject o = burningFrames[i];
            float t = framePercentages[i];

            if (t >= brokenPercent)
            {
                o.SetActive(true);
            }
        }

        myCollider.enabled = !broken;
    }

}