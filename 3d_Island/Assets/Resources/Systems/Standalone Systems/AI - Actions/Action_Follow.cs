using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Action_Follow : AbstractAction
{
    public Action_Follow(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Following " + subject.tag;
        priority = 2;
    }
    public override void Execute()
    {
        base.Execute();
        ServicesProvider.instance.StartCoroutine(Follow());
    }
    IEnumerator Follow()
    {
        float distance = 50f;

        while (distance >= myAgent.nearObjectDistance)
        {
            if (base.ShouldBreak())
                break;

            distance = (myAgent.transform.position - subject.transform.position).magnitude;
            
            if(myAgent.myAgent.isActiveAndEnabled)
                myAgent.myAgent.SetDestination(subject.transform.position);

            yield return new WaitForFixedUpdate();  
        }

        isDone = true;
    }
}
