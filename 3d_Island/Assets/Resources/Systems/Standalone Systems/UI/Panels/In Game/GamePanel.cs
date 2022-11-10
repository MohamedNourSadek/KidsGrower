using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamePanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Options").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Main0")));
        GetButton("Options").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(false)));
        GetButton("Options").onClick.AddListener(new UnityAction(() => GameManager.instance.SetCamera(false)));

        GetButton("Design").onClick.AddListener(new UnityAction(() => UIGame.instance.ToggleMenuPanel("Design Tools0")));


        GetButton("Inventory").onClick.AddListener(new UnityAction(() => GameManager.instance.SetBlur(true)));
        GetButton("Inventory").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(false)));
        GetButton("Inventory").onClick.AddListener(new UnityAction(() => GameManager.instance.OpenInventory(true)));
        GetButton("Inventory").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Inventory0")));
    }
}
