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

    public GameObject laserRayTarget = null;
    public LaserReciever targetedReciever = null;

    public bool deadly = false;

    public override void Start()
    {
        base.Start();
        laserSegment.SetActive(false);
    }

    // Start is called before the first frame update
    public override void Reset()
    {
        deadly = false;

        if (laserRayTarget != null)
        {
            LaserTargetable lt = laserRayTarget.GetComponent<LaserTargetable>();
            LaserReciever lr = laserRayTarget.GetComponent<LaserReciever>();
            if (lt != null)
            {
                lt.ResetTimer();
            }

            if (lr != null)
            {
                lr.LooseLaser();
            }
        }

        if (targetedReciever != null)
        {
            targetedReciever.LooseLaser();
        }

        laserRayTarget = null;
        targetedReciever = null;
    }

    public override void PhaseEvacuate()
    {
        deadly = true;
    }

    public override void PhasePlanning()
    {
        deadly = false;
    }

    public void TurnOn()
    {
        if (currentlyOn)
        {
            return;
        }

        //Game.DisplayFloatingText(transform.position, "On!");

        currentlyOn = true;
        laserRayTarget = null;
        UpdateLaserSegments();
    }

    public void TurnOff()
    {
        if (!currentlyOn)
        {
            return;
        }

        // Game.DisplayFloatingText(transform.position, "Offff!");
        currentlyOn = false;
        laserRayTarget = null;
        laserSegment.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (currentlyOn)
            UpdateLaserSegments();
    }

    public void UpdateLaserSegments()
    {
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

            Vector3 laserPos = laserStartPoint.transform.localPosition;
            laserPos.y = laserPos.y + distance / 2;
            laserPos.z = -5;
            laserSegment.transform.localPosition = laserPos;

            laserSprite.size = new Vector2(1, distance);
            var b = laserSprite.bounds;
            b.Encapsulate(laserSprite.bounds);
            laserCollider.offset = b.center - laserSegment.transform.position;
            laserCollider.size = new Vector2(0.5f, distance);

            GameObject newHit = usedHit.transform.gameObject;
            if (laserRayTarget != newHit)
            {
                if (newHit != null)
                {
                    LaserTargetable lt = newHit.GetComponent<LaserTargetable>();
                    if (lt != null)
                    {
                        if (targetedReciever != null && targetedReciever.gameObject != lt.gameObject)
                        {
                            if (targetedReciever != null)
                            {
                                lt.ResetTimer();
                            }
                        }
                    }

                    LaserReciever lr = newHit.GetComponent<LaserReciever>();
                    if (lr != null)
                    {
                        if (targetedReciever != lr || targetedReciever == null)
                        {
                            if (targetedReciever != null)
                            {
                                targetedReciever.LooseLaser();
                            }

                            targetedReciever = lr;
                            targetedReciever.RecieveLaser();
                        }
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
            if (lt != null && deadly)
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