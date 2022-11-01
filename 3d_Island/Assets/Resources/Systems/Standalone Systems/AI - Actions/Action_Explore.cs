using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Action_Explore : AbstractAction
{
    public Action_Explore(GameObject subject, NPC myAgent) : base(subject, myAgent)
    {
        actionName = "Exploring";
        priority = 1;
    }

    public override void Execute()
    {
        base.Execute();
        ServicesProvider.instance.StartCoroutine(Explore());
    }

    IEnumerator Explore()
    {
        Vector3 destination = MapSystem.instance.GetRandomExplorationPoint();

        if (myAgent.navMeshAgent.isActiveAndEnabled)
            myAgent.navMeshAgent.destination = destination;

        float distance = 50f;

        while (distance >= myAgent.nearObjectDistance)
        {
            if (base.ShouldBreak())
                break;

            distance = (myAgent.transform.position - destination).magnitude;

            if (myAgent.navMeshAgent.isActiveAndEnabled)
                myAgent.navMeshAgent.SetDestination(destination);

            yield return new WaitForFixedUpdate();
        }


        isDone = true;
    }
}
