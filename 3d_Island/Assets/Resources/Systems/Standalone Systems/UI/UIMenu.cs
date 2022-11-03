using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour
{
    [SerializeField] PanelsManager panelsManager;

    public static UIMenu instance;

    //private Functions
    private void Start()
    {
        instance = this;
        panelsManager.Initialize();
    }
    void OnDrawGizmos()
    {
        panelsManager.OnDrawGizmos();
    }


    //UI commands for buttons
    public void OpenMenuPanel(string _menuName)
    {
        panelsManager.OpenMenuPanel(_menuName, true);
    }
}





