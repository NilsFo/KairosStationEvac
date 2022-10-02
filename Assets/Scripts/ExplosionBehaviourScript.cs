using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExplosionBehaviourScript : Phaseable
{
    [SerializeField] private GameObject Indicator;
    [SerializeField] private GameObject Explosion;
    
    [SerializeField] private float InitIndicatorTime = 0f;
    [SerializeField] private float InitExplodeTime = 4f;

    [SerializeField] private float localTime = 0f;
    [SerializeField] private bool isOnFire = false;
    [SerializeField] private bool isLive = false;
    
    private List<CrewmateController> _crewmateControllers = new List<CrewmateController>();
    public float explosionMagnitude = 0.05f;
    public float explosionDuration = 0.3f;

    public override void Reset()
    {
        isLive = true;
        localTime = InitIndicatorTime + InitExplodeTime;
        isOnFire = false;
        Indicator.SetActive(false);
        Explosion.SetActive(false);
    }

    public override void PhaseEvacuate()
    {
        isLive = true;
        localTime = InitIndicatorTime + InitExplodeTime;
        isOnFire = false;
        Indicator.SetActive(false);
        Explosion.SetActive(false);
        GetComponent<Collider2D>().enabled = true;
    }

    public override void PhasePlanning()
    {
        isLive = false;
        localTime = 0;
        isOnFire = false;
        Indicator.SetActive(false);
        Explosion.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
    }

    private void Update()
    {
        if(!isLive) return;
        if (localTime > 0)
        {
            localTime -= Time.deltaTime;
        }
        if(localTime < InitExplodeTime && localTime > 0)
        {
            DisplayIndicator();
        }
        else if(localTime <= 0)
        {
            Explode();
        }
    }

    public void Explode()
    {
        for (int i = _crewmateControllers.Count - 1; i >= 0; i--)
        {
            var crew = _crewmateControllers[i];
            crew.Kill();
            _crewmateControllers.RemoveAt(i);
        }
        if(!isOnFire) {
            Game.ShakeCamera(explosionMagnitude, explosionDuration);
        }
        isOnFire = true;
        Indicator.SetActive(false);
        Explosion.SetActive(true);
        
    }

    public void DisplayIndicator()
    {
        Indicator.SetActive(true);
        Explosion.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        CrewmateController controller = col.gameObject.GetComponent<CrewmateController>();
        if(controller == null) return;
        if (isOnFire)
        {
            controller.Kill();
            return;
        }
        _crewmateControllers.Add(controller);
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        CrewmateController controller = col.gameObject.GetComponent<CrewmateController>();
        if(controller == null) return;
        _crewmateControllers.Remove(controller);
    }
}
