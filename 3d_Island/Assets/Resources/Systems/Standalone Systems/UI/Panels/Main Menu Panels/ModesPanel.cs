using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModesPanel : MenuPanel
{
    [SerializeField] SavesPanel savesPanel;

    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Free Mode").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Game Data")));
        GetButton("Free Mode").onClick.AddListener(new UnityAction(() => savesPanel.SetLastScene("Game Data")));
        GetButton("Free Mode").onClick.AddListener(new UnityAction(() => savesPanel.SetCurrentMode("FreeMode")));
        GetButton("Free Mode").onClick.AddListener(new UnityAction(() => savesPanel.UpdateSavesUI()));

        GetButton("Under Pop").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Game Data")));
        GetButton("Under Pop").onClick.AddListener(new UnityAction(() => savesPanel.SetLastScene("Game Data")));
        GetButton("Under Pop").onClick.AddListener(new UnityAction(() => savesPanel.SetCurrentMode("UnderPopulation")));
        GetButton("Under Pop").onClick.AddListener(new UnityAction(() => savesPanel.UpdateSavesUI()));

        GetButton("Back").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Main")));
    }
}
