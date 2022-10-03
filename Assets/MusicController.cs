using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
    private GameState _state;
    public AudioSource musicEvac, musicPlan, bigExplosion;
    
    
    public bool evac;
    public float fadeSpeed = 4f;
    public float maxVolume = 0.5f;
    
    private float _fade;
    
    // Start is called before the first frame update
    void Start() {
        _state = FindObjectOfType<GameState>();
        musicEvac.volume = _fade * maxVolume;
        musicPlan.volume = (1 - _fade) * maxVolume;
    }


    // Update is called once per frame
    void Update() {
        evac = _state.currentPhase == GameState.Phase.ExplosionPhase || _state.currentPhase == GameState.Phase.EvacuationPhase;
        if (evac) {
            if (_fade < 1) {
                _fade += fadeSpeed * Time.deltaTime;
                if (_fade > 1)
                    _fade = 1;
                musicEvac.volume = _fade * maxVolume;
                musicPlan.volume = (1 - _fade) * maxVolume;
            }
        } else {
            if (_fade > 0) {
                _fade -= fadeSpeed * Time.deltaTime;
                if (_fade < 0) {
                    _fade = 0;
                }
                musicEvac.volume = _fade * maxVolume;
                musicPlan.volume = (1 - _fade) * maxVolume;
            }
            
        }
        if (_state.currentPhase == GameState.Phase.ExplosionPhase) {
            if(!bigExplosion.isPlaying)
                bigExplosion.Play();
        } else {
            bigExplosion.Stop();
        }
    }


}
