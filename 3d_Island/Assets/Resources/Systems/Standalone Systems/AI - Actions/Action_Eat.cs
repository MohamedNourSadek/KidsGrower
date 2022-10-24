using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


[System.Serializable]
public class Action_Eat : AbstractAction
{
    public Action_Eat(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Eating " + subject.tag;
        priority = 2;
    }
    public override void Execute()
    {
        if (subject != null && myAgent.detector.IsNear(subject))
        {
            base.Execute();
            ServicesProvider.instance.StartCoroutine(Eat());
        }
        else
        {
            isDone = true;
        }

    }
    IEnumerator Eat()
    {
        bool willEat = false;

        if (subject.GetComponent<Pickable>())
        {
            myAgent.handSystem.PickObject(subject.GetComponent<Pickable>());

            if (myAgent.GotEatableInHand())
                willEat = true;
        }

        if(willEat)
        {
            ConditionChecker condition = new ConditionChecker(!(myAgent.isPicked));
            Eatable eatable = subject.GetComponent<Eatable>();
            UIGame.instance.ShowRepeatingMessage("Eating", myAgent.transform, 15, 15, condition);

            while (condition.isTrue)
            {
                if (base.ShouldBreak())
                    break;

                bool eatableInHand = myAgent.GotEatableInHand();
                bool hasMore = eatable.HasMore();

                condition.Update(eatableInHand && hasMore);

                eatable.ApplyEffect(myAgent);

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }

            myAgent.handSystem.DropObject();

            condition.Update(false);
            isDone = true;
        }
        else
        {
            isDone = false;
        }

    }
}
