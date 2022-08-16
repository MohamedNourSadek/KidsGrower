using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour, IPanelsManagerUser
{
    [SerializeField] string defaultPanel;
    [SerializeField] PanelsManager panelsManager;
    [SerializeField] BackgroundAnimation backgroundAnimation;

    void Start()
    {
        backgroundAnimation.Initialize();
        panelsManager.Initialize(defaultPanel);
    }
    void OnDrawGizmos()
    {
        panelsManager.OnDrawGizmos();
    }


    public void OpenMenuPanel(string _menuName)
    {
        panelsManager.OpenMenuPanel(_menuName);
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





