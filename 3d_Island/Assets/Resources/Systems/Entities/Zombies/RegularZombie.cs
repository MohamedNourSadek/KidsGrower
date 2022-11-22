using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularZombie : Zombie
{
    private void FixedUpdate()
    {
        if (detector.GetNearestInRange() != null)
        {
            myAgent.SetDestination(detector.GetNearestInRange().GetGameObject().transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(myAgent.destination, 0.5f);
        Gizmos.DrawLine(this.transform.position, myAgent.destination);
    }
}
