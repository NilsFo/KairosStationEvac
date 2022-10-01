using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxEntity : Phaseable
{
    private Vector3 _initialPosition;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Reset() {
        transform.position = _initialPosition;
    }
    public override void PhaseEvacuate() {
    }
    public override void PhasePlanning() {
    }
}
