using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCStatsPanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Full Button").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("FullStats0")));
        GetButton("Full Button").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(false)));
        GetButton("Full Button").onClick.AddListener(new UnityAction(() => PlayerSystem.instance.UpdateFullStats()));

        GetButton("Done").onClick.AddListener(new UnityAction(() => GameManager.instance.OnNamingFinsihed()));
    }
}
