using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsPanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        if (UIMenu.instance != null)
        {
            GetButton("Audio").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Audio")));
            GetButton("Graphics").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Graphics")));
            GetButton("Back").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Main")));
        }
        else if(UIGame.instance != null) 
        {
            GetButton("Audio").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Audio0")));
            GetButton("Graphics").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Graphics0")));
            GetButton("Back").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Main0")));
        }
    }

}