using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderPopulation : AbstractMode
{
    public UnderPopulation(Mode_Data data) : base(data)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
    }
    protected override void OnStart()
    {
        base.OnStart();

        ServicesProvider.instance.StartCoroutine(startEnum());
    }

    IEnumerator startEnum()
    {
        gameManager.LockPlayer(true);

        yield return new WaitForSeconds(1f);

        controller.ShowUIMessage("Under Population", 1.5f ,new Vector3(1,1,1), 1f);

        yield return new WaitForSeconds(1.5f);

        controller.ShowUIMessage("3", 1f, new Vector3(1, 1, 1), 1f);
        yield return new WaitForSeconds(1f);
        controller.ShowUIMessage("2", 1f, new Vector3(1, 1, 1), 1f);
        yield return new WaitForSeconds(1f);
        controller.ShowUIMessage("1", 1f, new Vector3(1, 1, 1), 1f);
        gameManager.SpawnEgg();
        yield return new WaitForSeconds(1f);
        controller.ShowUIMessage("Go !", 1f, new Vector3(1, 1, 1), 1f);

        gameManager.LockPlayer(false);

    }


}
