using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserTargetable : MonoBehaviour
{

    public float breakTime = 1.337f;
    private float _breakTimeCurrent = 0;
    public bool Broken => _breakTimeCurrent >= breakTime;
    public UnityEvent brokenByLaser;
    
    // Start is called before the first frame update
    void Start()
    {
        if (brokenByLaser == null) brokenByLaser = new UnityEvent();
        ResetTimer();
    }

    public void Frizzle(float amount)
    {
        _breakTimeCurrent += amount;
    }

    public void ResetTimer()
    {
        _breakTimeCurrent = 0;
    }

    public void Break()
    {
        brokenByLaser.Invoke();
    }
    
}
