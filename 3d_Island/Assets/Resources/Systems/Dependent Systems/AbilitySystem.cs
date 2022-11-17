using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AbilitySystem
{
    public bool canStore;
    public bool canShake;
    public bool canPick;
    public bool canThrow;
    public bool canPlant;
    public bool canPet;
    public bool canTear;
    public bool canJump;
    public bool canDash;

    DetectorSystem detector;
    HandSystem hand;
    MovementSystem movementSystem;

    public void Initialize(DetectorSystem detector, HandSystem hand, MovementSystem movement)
    {
        this.detector = detector;
        this.hand = hand;
        
        if(movement != null)
            this.movementSystem = movement;
    }
    public void Update()
    {
        canShake = (detector.GetNear("Tree") != null);

        canTear = ((detector.GetNear("Tree") != null) || (detector.GetNear("Rock") != null)) && (hand.GetObjectInHand() != null) && hand.GetObjectInHand().tag == "Axe";

        if (movementSystem != null)
            canJump = movementSystem.IsOnGround();

        if(movementSystem !=null)
            canDash = movementSystem.IsDashable();

        canStore = (hand.nearPickables.Count >= 1) && (hand.nearPickables[0].GetComponent<IStorableObject>() != null);

        if (hand.GetObjectInHand() == null)
        {
            canPick = hand.nearPickables.Count >= 1;

            canThrow = false;
            canPlant = false;
        }
        else
        {
            canPick = false;
            canThrow = true;
        }

        if (hand.GetObjectInHand() && hand.GetObjectInHand().GetComponent<Plantable>())
            canPlant = hand.GetObjectInHand().GetComponent<Plantable>().IsOnPlatingGround(detector.transform.position);

        canPet = (detector.GetNear("NPC") != null) && (hand.GetObjectInHand() == null) && (hand.isPetting == false);
    }
}
