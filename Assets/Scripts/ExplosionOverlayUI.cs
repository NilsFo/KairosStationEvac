using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionOverlayUI : Phaseable
{
    public Image myImage;
    public CanvasRenderer canvasRenderer;
    public GameObject faderGameObject;

    public bool fading;
    public float fadeTime = 1.0f;
    public float fadeDelay = 0.69f;
    private float _fadeTimeCurrent;
    private float _fadeDelayCurrent;

    // Start is called before the first frame update
    public override void Reset()
    {
        fading = false;
    }

    public override void PhaseEvacuate()
    {
        fading = false;
    }

    public override void PhasePlanning()
    {
        fading = false;
    }

    public override void Start()
    {
        base.Start();
        fading = false;
        faderGameObject.SetActive(true);
    }

    public override void PhaseExplosion()
    {
        base.PhaseExplosion();
        fading = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            print("Fading. Delay: " + _fadeDelayCurrent + ". Fade: " + _fadeTimeCurrent);

            _fadeDelayCurrent = _fadeDelayCurrent + Time.deltaTime;
            if (_fadeDelayCurrent > fadeDelay)
            {
                _fadeTimeCurrent = _fadeTimeCurrent + Time.deltaTime;
                float fadePercent = _fadeTimeCurrent / fadeTime;
                fadePercent = Mathf.Min(fadePercent, 1.0f);
                canvasRenderer.SetAlpha(fadePercent);
            }
        }
        else
        {
            canvasRenderer.SetAlpha(0.0f);
            _fadeTimeCurrent = 0;
            _fadeDelayCurrent = 0;
        }
    }
}