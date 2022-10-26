using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Action_Throw : AbstractAction
{
    public Action_Throw(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Throwing On " + subject.tag;
        priority = 2;
    }

    public override void Execute()
    {
        base.Execute();

        ServicesProvider.instance.StartCoroutine(Throw());
    }

    IEnumerator Throw()
    {
        if(myAgent.handSystem.GetObjectInHand())
            myAgent.handSystem.ThrowObjectInHand((subject.transform.position));

        yield return new WaitForSeconds(1f);

        isDone = true;
    }
}
