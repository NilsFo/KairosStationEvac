using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExplosionBehaviourScript : Phaseable
{
    [SerializeField] private GameObject Indicator;
    [SerializeField] private GameObject Explosion;
    
    [SerializeField] private float InitIndicatorTime = 1f;
    [SerializeField] private float InitExplodeTime = 4f;
    
    [SerializeField] private float localTime = 0f;
    [SerializeField] private bool isOnFire = false;
    
    private List<CrewmateController> _crewmateControllers = new List<CrewmateController>();

    public override void Reset()
    {
        localTime = InitIndicatorTime + InitExplodeTime;
        isOnFire = false;
        Indicator.SetActive(false);
        Explosion.SetActive(false);
    }

    public override void PhaseEvacuate()
    {
        localTime = 0;
        isOnFire = false;
        Indicator.SetActive(false);
        Explosion.SetActive(false);
    }

    public override void PhasePlanning()
    {
        localTime = 0;
        isOnFire = false;
        Indicator.SetActive(false);
        Explosion.SetActive(false);
    }

    private void Update()
    {
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
        for (int i = 0; i < _crewmateControllers.Count; i++)
        {
            var crew = _crewmateControllers[i];
            //TODO Crew kill
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
            //TODO Crew kill
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
