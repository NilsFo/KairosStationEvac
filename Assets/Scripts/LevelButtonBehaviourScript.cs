using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject normal;
    [SerializeField] private GameObject highlighted;

    [SerializeField] private string levelName;
    
    private void Start()
    {
        normal.SetActive(true);
        highlighted.SetActive(false);
    }

    private void OnMouseEnter()
    {
        normal.SetActive(false);
        highlighted.SetActive(true);
    }

    private void OnMouseExit()
    {
        normal.SetActive(true);
        highlighted.SetActive(false);
    }

    private void OnMouseDown()
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
}
