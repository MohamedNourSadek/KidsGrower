using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Drop : AbstractAction
{
    public Action_Drop(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Droping " + subject.tag;
        priority = 2;
    }

    public override void Execute()
    {
        base.Execute();

        myAgent.handSystem.DropObject();

        isDone = true;
    }


}
