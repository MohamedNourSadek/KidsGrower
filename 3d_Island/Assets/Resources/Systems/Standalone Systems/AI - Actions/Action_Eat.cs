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
        base.Execute();

        ServicesProvider.instance.StartCoroutine(Eat());
    }
    IEnumerator Eat()
    {
        bool willEat = false;

        if (myAgent.handSystem.GetNearest())
        {
            if ((myAgent.handSystem.GetNearest()).tag == "Fruit")
            {
                myAgent.handSystem.PickObject();

                if ((Fruit)(myAgent.handSystem.GetObjectInHand()))
                    willEat = true;
            }
        }

        if(willEat)
        {
            ConditionChecker condition = new ConditionChecker(!(myAgent.isPicked));
            Fruit fruit = subject.GetComponent<Fruit>();
            UIGame.instance.ShowRepeatingMessage("Eating", myAgent.transform, 15, 15, condition);

            while (condition.isTrue)
            {
                if (base.ShouldBreak())
                    break;

                bool fruitInHand = myAgent.GotTypeInHand(typeof(Fruit));
                bool hasEnergy = fruit.HasEnergy();

                condition.Update(fruitInHand && hasEnergy);

                myAgent.levelController.IncreaseXP(fruit.GetEnergy());

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
