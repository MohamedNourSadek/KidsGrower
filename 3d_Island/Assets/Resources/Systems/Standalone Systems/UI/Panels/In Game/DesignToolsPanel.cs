using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DesignToolsPanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Egg").onClick.AddListener(new UnityAction(() => GameManager.instance.SpawnEgg()));
        GetButton("Ball").onClick.AddListener(new UnityAction(() => GameManager.instance.SpawnBall()));
        GetButton("Seed").onClick.AddListener(new UnityAction(() => GameManager.instance.SpawnSeed()));
    }
}
