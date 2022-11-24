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
        ServicesProvider.instance.StartCoroutine(SpawnEveryT());
    }
    protected override void OnFirstLoad()
    {
        base.OnFirstLoad();
        GameManager.instance.SpawnAxe();
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


