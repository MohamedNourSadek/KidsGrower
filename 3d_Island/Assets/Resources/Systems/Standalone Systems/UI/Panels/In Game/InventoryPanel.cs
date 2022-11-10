using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryPanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Back").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Game0")));
        GetButton("Back").onClick.AddListener(new UnityAction(() => GameManager.instance.SetBlur(false)));
        GetButton("Back").onClick.AddListener(new UnityAction(() => GameManager.instance.OpenInventory(false)));
        GetButton("Back").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(true)));
    }

}
