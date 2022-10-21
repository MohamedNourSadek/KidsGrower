using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeMode : AbstractMode
{
    public FreeMode(Mode_Data data) : base(data)
    {

    }

    protected override void OnLoad()
    {
        base.OnLoad();

        if (!data.gameStarted)
        {
            data.timeSinceStart = 0;
            OnFirstStart();
        }

    }
    protected override void OnFirstStart()
    {
        base.OnFirstStart();
        ServicesProvider.instance.StartCoroutine(StartLogic());
    }
    IEnumerator StartLogic()
    {
        yield return null;

        gameManager.SpawnEgg();
        gameManager.SpawnRandomBoosts();
    }
}


