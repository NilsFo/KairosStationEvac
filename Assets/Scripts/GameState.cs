using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    public enum Phase
    {
        Unknown,
        EvacuationPhase,
        PlanningPhase,
        ExplosionPhase,
        WinState
    }

    private bool firstCleanDone = false;
    public Phase currentPhase = Phase.Unknown;
    private Phase _lastKnownPhase;
    public UnityEvent onPhasePlaying;
    public UnityEvent onPhasePlanning;
    public UnityEvent onWin;
    public UnityEvent onExplosion;
    public UnityEvent onResetGameplay;

    private List<Phaseable> myObservers = new List<Phaseable>();
    private List<CrewmateController> allCrewmates = new List<CrewmateController>();
    public CrewmateController selectedCrewmate;
    public int CrewmateCount => allCrewmates.Count;

    public GameObject floatingTextPrefab;

    // Camera Shake
    public GameObject CMCameraFocus;
    public float cameraShakeMagnitude = 0f;
    public float cameraShakeDuration = 0f;
    private float _cameraShakeDurationTimer = 0f;

    private void OnEnable()
    {
        var crewmates = FindObjectsOfType<CrewmateController>();
        foreach (CrewmateController c in crewmates)
        {
            allCrewmates.Add(c);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (onPhasePlaying == null) onPhasePlaying = new UnityEvent();
        if (onPhasePlanning == null) onPhasePlanning = new UnityEvent();
        if (onWin == null) onWin = new UnityEvent();
        if (onResetGameplay == null) onResetGameplay = new UnityEvent();
        if (onExplosion == null) onExplosion = new UnityEvent();

        CMCameraFocus = GameObject.FindGameObjectWithTag("CameraFocus");
        ResetCameraShake();
    }

    public void SubscribeToPhases(Phaseable p)
    {
        if (!myObservers.Contains(p))
        {
            myObservers.Add(p);
        }
    }

    public void UnsubscribeFromPhases(Phaseable p)
    {
        if (myObservers.Contains(p))
        {
            myObservers.Remove(p);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_lastKnownPhase != currentPhase)
        {
            OnStateChanged();
            _lastKnownPhase = currentPhase;
        }

        if (cameraShakeMagnitude <= 0 || _cameraShakeDurationTimer >= cameraShakeDuration)
        {
            ResetCameraShake();
        }

        if (cameraShakeDuration > 0)
        {
            _cameraShakeDurationTimer += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = CMCameraFocus.transform.position;
        pos.x = 0;
        pos.y = 0;

        if (currentPhase == Phase.ExplosionPhase)
        {
            // print("EXPL!");
        }

        if (cameraShakeDuration > 0 && _cameraShakeDurationTimer < cameraShakeDuration)
        {
            // Shaking it!
            pos.x = Random.Range(cameraShakeMagnitude * -1, cameraShakeMagnitude);
            pos.y = Random.Range(cameraShakeMagnitude * -1, cameraShakeMagnitude);
            // Debug.Log("Camera shake!", CMCameraFocus);
        }

        CMCameraFocus.transform.localPosition = pos;
    }

    public int GetRescuedCrewmateCount()
    {
        int rescueCount = 0;
        foreach (CrewmateController crewmateController in allCrewmates)
        {
            if (crewmateController.rescued)
            {
                rescueCount++;
            }
        }

        return rescueCount;
    }

    public void CheckWinCondition()
    {
        int rescueCount = GetRescuedCrewmateCount();
        if (rescueCount == CrewmateCount)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        selectedCrewmate = null;
        print("A winner is you!");
        currentPhase = Phase.WinState;
    }

    private void OnStateChanged()
    {
        print("The state has changed. From: '" + _lastKnownPhase + "' to '" + currentPhase);
        switch (currentPhase)
        {
            case Phase.PlanningPhase:
                OnPhasePlanning();
                break;
            case Phase.EvacuationPhase:
                OnPhaseEvacuate();
                break;
            case Phase.WinState:
                OnPhaseWin();
                break;
            case Phase.ExplosionPhase:
                OnPhaseExplosion();
                break;
        }
    }

    private void OnPhaseEvacuate()
    {
        onPhasePlaying.Invoke();

        // Setting player control
        foreach (CrewmateController c in allCrewmates)
        {
            c.playerControlled = false;
        }

        if (selectedCrewmate != null)
        {
            selectedCrewmate.playerControlled = true;
        }

        foreach (var p in myObservers)
        {
            p.PhaseEvacuate();
        }
    }

    private void OnPhasePlanning()
    {
        ResetAndCleanUp();
        onPhasePlanning.Invoke();
        foreach (var p in myObservers)
        {
            p.PhasePlanning();
        }
    }

    private void OnPhaseWin()
    {
        onWin.Invoke();
        foreach (var p in myObservers)
        {
            p.PhaseWin();
        }
    }

    private void OnPhaseExplosion()
    {
        onExplosion.Invoke();
        foreach (var p in myObservers)
        {
            p.PhaseExplosion();
        }
    }

    private void ResetAndCleanUp()
    {
        onResetGameplay.Invoke();
        selectedCrewmate = null;
        ResetCameraShake();

        for (var index = myObservers.Count - 1; index >= 0; index--) {
            var p = myObservers [index];
            p.Reset();
        }
    }

    public void BackToMainMenu()
    {
        print("Back to main menu.");
        // TODO implement
    }

    public void DisplayFloatingText(Vector3 position, string text, float duration = 3f, float fontSize = 0.69f,
        float velocity_Y = 0.15f, float velocity_Z = 0f, float velocity_X = 0f)
    {
        var newObj = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        FloatingText flt = newObj.GetComponent<FloatingText>();
        flt.text = text;
        flt.duration = duration;
        flt.velocity.y = velocity_Y;
        flt.velocity.x = velocity_X;
        flt.velocity.z = velocity_Z;
        flt.fontSize = fontSize;
    }

    public void ShakeCamera(float magnitude, float duration)
    {
        //print("Request to shake the camera by " + magnitude + " for " + duration);
        if (magnitude >= cameraShakeMagnitude)
        {
            //print("Request accepted.");
            cameraShakeMagnitude = magnitude;
            cameraShakeDuration = duration;
        }
        else
        {
            //print("Magnitude too low. Denied.");
        }
    }

    public void ResetCameraShake()
    {
        cameraShakeMagnitude = 0;
        cameraShakeDuration = 0;
        _cameraShakeDurationTimer = 0;
    }
}