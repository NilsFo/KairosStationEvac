using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : Phaseable
{
    public Transform laserStartPoint;

    public bool initiallyOn = true;
    private bool currentlyOn = false;
    public GameObject laserSegment;
    public SpriteRenderer laserSprite;
    public BoxCollider2D laserCollider;

    public GameObject laserRayTarget;
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

    private void FixedUpdate()
    {
        if (currentlyOn && Game.currentPhase == GameState.Phase.EvacuationPhase)
            UpdateLaserSegments();
    }

    public void UpdateLaserSegments()
    {
        if (Game.currentPhase != GameState.Phase.EvacuationPhase)
            return;
        laserRayTarget = null;

        int layerDefault = LayerMask.GetMask("Default");
        int layerCrewmates = LayerMask.GetMask("Crewmates");
        Vector3 rot = transform.rotation * Vector3.up;
        RaycastHit2D hitDefault = Physics2D.Raycast(laserStartPoint.position, rot, 1000f, layerDefault);
        // RaycastHit2D hitCrewmate = Physics2D.Raycast(laserStartPoint.position, rot, 1000f, layerCrewmates);

        RaycastHit2D usedHit = hitDefault;
        // if (hitDefault.collider == null)
        // {
        //     usedHit = hitCrewmate;
        // }
        // else if (hitCrewmate.collider == null)
        // {
        //     usedHit = hitDefault;
        // }
        // else
        // {
        //     float distanceDefault = hitDefault.distance;
        //     float distanceCrewmate = hitCrewmate.distance;
        //     if (distanceDefault < distanceCrewmate)
        //     {
        //         usedHit = hitDefault;
        //     }
        //     else
        //     {
        //         usedHit = hitCrewmate;
        //     }
        // }

        if (usedHit.collider != null)
        {
            float distance = Mathf.Abs(usedHit.point.y - transform.position.y);
            distance = usedHit.distance;
            var destination = usedHit.point;
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
            laserCollider.size = new Vector2(0.5f, distance);

            GameObject newHit = usedHit.transform.gameObject;
            if (laserRayTarget != newHit)
            {
                if (laserRayTarget != null)
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
        if (laserRayTarget != null)
        {
            LaserTargetable lt = laserRayTarget.GetComponent<LaserTargetable>();
            if (lt != null)
            {
                lt.Frizzle(Time.deltaTime);
                if (lt.Broken)
                {
                    UpdateLaserSegments();
                }
            }
        }
    }
}