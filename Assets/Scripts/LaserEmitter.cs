using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : Phaseable
{
    
    public bool initiallyOn = true;
    private bool currentlyOn = false;
    public GameObject laserSegment;
    public SpriteRenderer laserSprite;
    public BoxCollider2D laserCollider;

    private GameObject laserRayTarget;
    private GameObject _lastKnownRayTarget;

    public override void Start()
    {
        laserSegment.SetActive(false);
        base.Start();
        Reset();
    }

    // Start is called before the first frame update
    public override void Reset()
    {
        if (initiallyOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }

    public override void PhaseEvacuate()
    {
    }

    public override void PhasePlanning()
    {
    }

    public void TurnOn()
    {
        if (currentlyOn)
        {
            return;
        }

        currentlyOn = true;
        laserRayTarget = null;
        _lastKnownRayTarget = null;
        UpdateLaserSegments();
    }

    public void TurnOff()
    {
        if (!currentlyOn)
        {
            return;
        }

        currentlyOn = false;
        laserRayTarget = null;
        _lastKnownRayTarget = null;
        laserSegment.SetActive(false);
    }

    private void FixedUpdate() {
        if(currentlyOn && Game.currentPhase == GameState.Phase.EvacuationPhase)
            UpdateLaserSegments();
    }

    public void UpdateLaserSegments() {
        if (Game.currentPhase != GameState.Phase.EvacuationPhase)
            return;
        laserRayTarget = null;
        
        int layer = LayerMask.GetMask("Default");
        Vector3 rot = transform.rotation * Vector3.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rot, 1000f, layer);
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            distance = hit.distance;
            var destination = hit.point;
            //Debug.Log("hit", hit.transform.gameObject);
            //Debug.Log("Distance: "+distance);

            Vector3 laserPos = new Vector3();
            laserPos.y = laserPos.y + distance / 2;
            laserPos.z = 0.1337f;
            laserSegment.transform.localPosition = laserPos;

            laserSprite.size = new Vector2(1, distance);
            var b = laserSprite.bounds;
            b.Encapsulate(laserSprite.bounds);
            laserCollider.offset = b.center - laserSegment.transform.position;
            laserCollider.size = new Vector2(0.5f,distance);

            GameObject newHit = hit.transform.gameObject;
            if (laserRayTarget!=newHit)
            {
                if (laserRayTarget!=null)
                {
                    LaserTargetable lt = laserRayTarget.GetComponent<LaserTargetable>();
                    if (lt != null)
                    {
                        lt.ResetTimer();
                    }
                }
            }

            laserRayTarget = newHit;
        }
        else
        {
            Debug.Log("Infinite laser!");
        }

        laserSegment.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (laserRayTarget!=null)
        {
            LaserTargetable lt = laserRayTarget.GetComponent<LaserTargetable>();
            if (lt != null)
            {
                lt.Frizzle(Time.deltaTime);
                if (lt.Broken)
                {
                    lt.Break();
                    UpdateLaserSegments();
                }
            }
        }
    }
}