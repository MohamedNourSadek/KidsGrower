using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public class Action_Lay : AbstractAction
{
    public Action_Lay(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Laying " + subject.tag;
        priority = 2;
    }
    public override void Execute()
    {
        if (subject != null && myAgent.detector.IsNear(subject))
        {       
            base.Execute();
            ServicesProvider.instance.StartCoroutine(Lay());
        }
        else
        {
            isDone = true;
        }

    }
    IEnumerator Lay()
    {
        if (myAgent.character.CanLay())
        {
            float time = 0;
            ConditionChecker condition = new ConditionChecker(!myAgent.isPicked);
            UIGame.instance.ShowRepeatingMessage("Laying", myAgent.transform, myAgent.character.layingTime, 15, condition);

            //Laying
            myAgent.GetBody().isKinematic = true;
            myAgent.navMeshAgent.enabled = false;
            while (condition.isTrue)
            {
                if (base.ShouldBreak())
                    break;

                time += Time.fixedDeltaTime;

                condition.Update((time <= myAgent.character.layingTime));

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }

            //Done
            if (!myAgent.isPicked)
            {
                myAgent.navMeshAgent.enabled = true;
                myAgent.GetBody().isKinematic = false;
            }

            if (time >= myAgent.character.layingTime)
            {
                Egg egg = GameManager.instance.SpawnEgg_ReturnEgg(myAgent.transform.position + Vector3.up);

                egg.SetRottenness(1f - myAgent.character.GetFertility());

                myAgent.character.lastLaidSince = 0f;
            }

            condition.Update(false);
        }

        isDone = true;

    }
}


public class random
{
    public int id;
    public int name;
    public int des;




}
