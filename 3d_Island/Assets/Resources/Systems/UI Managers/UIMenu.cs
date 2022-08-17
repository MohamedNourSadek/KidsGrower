using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour, IPanelsManagerUser
{
    [SerializeField] PanelsManager panelsManager;
    [SerializeField] BackgroundAnimation backgroundAnimation;

    void Start()
    {
        backgroundAnimation.Initialize();
        panelsManager.Initialize();
    } 
    void OnDrawGizmos()
    {
        panelsManager.OnDrawGizmos();
    }


    public void OpenMenuPanel(string _menuName)
    {
        panelsManager.OpenMenuPanel(_menuName, false);
    }
    public void ToggleMenuPanel(string _menuInfo)
    {
        panelsManager.ToggleMenuPanel(_menuInfo, false);
    }
    public void OpenMenuPanelNonExclusive(string _menuInfo)
    {
        panelsManager.OpenMenuPanel(_menuInfo, true);
    }
    public void CloseMenuPanelNonExclusive(string _menuInfo)
    {
        panelsManager.CloseMenuPanel(_menuInfo);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OpenGame()
    {
        SceneManager.LoadSceneAsync(1);
    }


}





