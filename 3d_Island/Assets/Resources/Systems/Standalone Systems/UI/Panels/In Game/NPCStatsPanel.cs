using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPCStatsPanel : MenuPanel
{

    Button drop;
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Full Button").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("FullStats0")));
        GetButton("Full Button").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(false)));
        GetButton("Full Button").onClick.AddListener(new UnityAction(() => PlayerSystem.instance.UpdateFullStats()));
        GetButton("Done").onClick.AddListener(new UnityAction(() => GameManager.instance.OnNamingFinsihed()));

        drop = GetButton("Drop");
        drop.onClick.AddListener(new UnityAction(() => PlayerSystem.instance.GetNPCInHand().appearanceControl.DropHat()));
    }

    private void Update()
    {
        if(this.isActiveAndEnabled)
            drop.interactable = PlayerSystem.instance.GetNPCInHand().appearanceControl.hat != null;
    }
}
