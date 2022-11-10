using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuMainPanel : MenuPanel
{
    public override void FillFunctions()
    {
        GetButton("Play").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Modes")));
        GetButton("Settings").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Settings")));
        GetButton("Quit").onClick.AddListener(new UnityAction(() => MenuManager.instance.Quit()));
    }
}
