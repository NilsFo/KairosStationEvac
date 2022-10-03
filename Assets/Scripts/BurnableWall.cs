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

    public AudioSource breakSound;

    public float brokenPercent;
    public bool broken;
    public int frameVisible;

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
        base.Start();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (!broken && myLaserTarget.Broken) {
            breakSound.Play();
            Game.ShakeCamera(0.1f, 0.1f);
        }
        
        brokenPercent = myLaserTarget.GetDestructionPercent();
        broken = myLaserTarget.Broken;

        foreach (var o in burningFrames)
        {
            o.SetActive(false);
        }

        frameVisible = 0;
        for (int i = 0; i < framePercentages.Count; i++)
        {
            GameObject o = burningFrames[i];
            float t = framePercentages[i];

            if (t <= brokenPercent)
            {
                frameVisible = i;
            }
        }

        burningFrames[frameVisible].SetActive(true);
        myCollider.enabled = !broken;
    }
}