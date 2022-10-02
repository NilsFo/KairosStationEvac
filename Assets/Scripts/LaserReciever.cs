using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserReciever : Phaseable
{

    public bool hasLaserPower;
    public UnityEvent laserGet;
    public UnityEvent laserLost;
    public GameObject spriteOn;
    public GameObject spriteOff;
    
    public override void Start()
    {
        base.Start();
        if (laserGet == null) laserGet = new UnityEvent();
        if (laserLost == null) laserLost = new UnityEvent();
        Reset();
    }

    public void RecieveLaser()
    {
        hasLaserPower = true;
        laserGet.Invoke();
        
        if (Game != null)
        {
            Game.DisplayFloatingText(transform.position,"Laser Get!");
        }
    }

    public void LooseLaser()
    {
        hasLaserPower = false;
        laserLost.Invoke();
        
        if (Game != null)
        {
            Game.DisplayFloatingText(transform.position,"Laser Lost");
        }
    }
    
    // Start is called before the first frame update
    public override void Reset()
    {
        hasLaserPower = false;
        LooseLaser();
    }

    public override void PhaseEvacuate()
    {
    }

    public override void PhasePlanning()
    {
    }

    private void Update()
    {
        spriteOff.SetActive(!hasLaserPower);
        spriteOn.SetActive(hasLaserPower);
    }
}
