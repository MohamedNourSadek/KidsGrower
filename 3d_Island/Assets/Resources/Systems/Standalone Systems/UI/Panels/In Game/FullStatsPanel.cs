using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FullStatsPanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Close").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Game0")));
        GetButton("Close").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(true)));
        GetButton("Close").onClick.AddListener(new UnityAction(() => GameManager.instance.SetBlur(false)));
    }
}
