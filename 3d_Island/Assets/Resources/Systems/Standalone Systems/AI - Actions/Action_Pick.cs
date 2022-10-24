using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Pick : AbstractAction
{
    public Action_Pick(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Picking " + subject.tag;
        priority = 2;
    }

    public override void Execute()
    {
        if (subject != null && myAgent.detector.IsNear(subject))
        {
            base.Execute();
            ServicesProvider.instance.StartCoroutine(Picking());
        }
        else
        {
            isDone = true;
        }
    }

    IEnumerator Picking()
    {
        subject.GetComponent<Pickable>().Pick(myAgent.handSystem);

        yield return new WaitForSeconds(1f);

        isDone = true;
    }


}
