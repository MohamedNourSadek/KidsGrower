using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Shake : AbstractAction
{
    public Action_Shake(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Shaking " + subject.tag;
        priority = 2;
    }
    public override void Execute()
    {
        base.Execute();

        ServicesProvider.instance.StartCoroutine(Shake());
    }
    IEnumerator Shake()
    {
        subject.GetComponent<TreeSystem>().Shake();
        yield return new WaitForSeconds(1f);
        isDone = true;
    }
}
