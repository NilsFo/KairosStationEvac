using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialTextBox : MonoBehaviour
{
    private GameState game;
    private GamePhaseLoop _loop;
    public TutorialTextBox nextBox;
    public GameObject myBox;

    public float visibleDelay=1.1337f;
    private float _visibleTimer;
    private bool visibleRequested;
    public bool visible;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<GameState>();
        _loop = FindObjectOfType<GamePhaseLoop>();
        visible = false;
        _visibleTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        myBox.SetActive(visible);
        if (visibleRequested)
        {
            _visibleTimer += Time.deltaTime;
            if (_visibleTimer > visibleDelay)
            {
                visible = true;
            }
        }

        if (visible)
        {
            if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q))
            {
                HideAndShowNext();
            }
        }
    }

    public void Show()
    {
        if (!visible)
        {
            visibleRequested = true;
        }
    }

    private void Hide()
    {
        visible = false;
        visibleRequested = false;
    }

    public void HideAndShowNext()
    {
        Hide();
        if (nextBox == null)
        {
            _loop.SetPhasePlanning();
        }
        else
        {
            nextBox.Show();
        }
    }
}