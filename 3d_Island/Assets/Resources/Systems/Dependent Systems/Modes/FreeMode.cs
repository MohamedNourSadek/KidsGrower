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

        if (!data.firstStart)
        {
            data.timeSinceStart = 0;
            GameManager.instance.SpawnAxe();
            data.firstStart = true;
        }

        ServicesProvider.instance.StartCoroutine(SpawnEveryT());
    }
    IEnumerator SpawnEveryT()
    {
        var t = 120;

        while (true)
        {
            yield return new WaitForSecondsRealtime(t);
            GameManager.instance.SpawnRandomItem();
        }
    }
}


