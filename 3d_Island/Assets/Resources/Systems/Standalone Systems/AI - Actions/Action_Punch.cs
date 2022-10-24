using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Punch : AbstractAction
{
    public Action_Punch(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Punching " + subject.tag;
        priority = 2;
    }

    public override void Execute()
    {
        if (subject != null && myAgent.detector.IsNear(subject))
        {
            base.Execute();
            ServicesProvider.instance.StartCoroutine(Punch());
        }
        else
        {
            isDone = true;
        }
    }


    IEnumerator Punch()
    {
        Vector3 direction = (subject.transform.position - myAgent.transform.position).normalized;
        subject.GetComponent<Rigidbody>().AddForce(direction * myAgent.character.GetPunchForce(), ForceMode.VelocityChange);
        yield return new WaitForSeconds(1f);
        isDone = true;
    }
}
