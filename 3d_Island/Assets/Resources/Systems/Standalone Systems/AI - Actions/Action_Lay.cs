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
        base.Execute();

        ServicesProvider.instance.StartCoroutine(Lay());
    }
    IEnumerator Lay()
    {
        float time = 0;
        ConditionChecker condition = new ConditionChecker(!myAgent.isPicked);
        UIGame.instance.ShowRepeatingMessage("Laying", myAgent.transform, myAgent.layingTime, 15, condition);

        //Laying
        myAgent.GetBody().isKinematic = true;
        myAgent.myAgent.enabled = false;

        while (condition.isTrue)
        {
            if (base.ShouldBreak())
                break;

            time += Time.fixedDeltaTime;

            condition.Update((time <= myAgent.layingTime));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Done
        if (!myAgent.isPicked)
        {
            myAgent.myAgent.enabled = true;
            myAgent.GetBody().isKinematic = false;
        }

        if (time >= myAgent.layingTime)
        {
            Egg egg = GameManager.instance.SpawnEgg_ReturnEgg(myAgent.transform.position + Vector3.up);

            float maxFertilityAge = myAgent.maxFertilityAge;

            float maxValue = myAgent.levelController.GetLevelsCount() * maxFertilityAge;
            float currentValue = myAgent.levelController.GetLevel() * myAgent.bornSince;

            egg.SetRottenness(1f - (currentValue /maxValue));

            myAgent.lastLaidSince = 0f;
        }

        condition.Update(false);
        isDone = true;

    }
}
