using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Idle : AbstractAction
{
    public Action_Idle(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Idle";
        priority = 1;
    }


    public override void Execute()
    {
        base.Execute();
        ServicesProvider.instance.StartCoroutine(Idle());
    }

    IEnumerator Idle()
    {
        yield return new WaitForSeconds(3f);
        isDone = true;
    }
}
