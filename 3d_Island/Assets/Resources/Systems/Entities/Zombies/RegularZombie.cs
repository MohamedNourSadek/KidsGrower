using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularZombie : Zombie
{
    [SerializeField] int hitAmount = 5;
    [SerializeField] float attackReach = 1f;

    public bool attacking = false;
    HealthControl attackedHealth; 

    private void Awake()
    {
        detector.Initialize(attackReach, OnDetectableInRange, OnDetectableExit, OnDetectableNear, OnDetectableNearExit);
    }

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


    void OnDetectableInRange(IDetectable detectable)
    {
    }
    void OnDetectableExit(IDetectable detectable)
    {
    }
    void OnDetectableNear(IDetectable detectable)
    {
        if (!attacking)
        {
            if (detectable.tag == "Player")
            {
                PlayerSystem player = detectable.GetGameObject().GetComponent<PlayerSystem>();
                attackedHealth = player.healthControl;
                StartCoroutine(Attack(attackedHealth));
            }
            else if(detectable.tag == "NPC")
            {
                NPC npc = detectable.GetGameObject().GetComponent<NPC>();
                attackedHealth = npc.character.healthControl;
                StartCoroutine(Attack(attackedHealth));
            }
        }
    }
    void OnDetectableNearExit(IDetectable detectable)
    {
        if (detectable.tag == "Player")
        {
            PlayerSystem player = detectable.GetGameObject().GetComponent<PlayerSystem>();

            if(attackedHealth == player.healthControl)
            {
                attacking = false;
            }
        }
        else if (detectable.tag == "NPC")
        {
            NPC npc = detectable.GetGameObject().GetComponent<NPC>();

            if(attackedHealth == npc.character.healthControl)
            {
                attacking = false;
            }
        }
    }

    IEnumerator Attack(HealthControl healthControl)
    {
        attacking = true;
        
        while(attacking)
        {
            if (healthControl.GetMyEntity() != null)
            {
                healthControl.GetAttacked(hitAmount, this.transform);
            }
            else
            {
                attacking = false; 
                break;
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }
}
