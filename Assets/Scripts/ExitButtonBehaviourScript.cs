using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButtonBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject normal;
    [SerializeField] private GameObject highlighted;

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
        Application.Quit();
    }
}
