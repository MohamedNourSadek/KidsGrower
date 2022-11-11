using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable]
public class HandSystem
{
    [Header("Pickable Parameters")]
    [SerializeField] GameObject myHand;
    [SerializeField] bool highlightToPick;
    [SerializeField] List<string> pickableTags = new List<string>();
    [SerializeField] float throwForce = 20f;
    [SerializeField] float pickSpeedThrushold = 2f;
    [SerializeField] float petTime = 1f;
     
    public bool gotSomethingInHand;
    public bool canStore;
    public bool canShake;
    public bool canPick;
    public bool canThrow;
    public bool canPlant; 
    public bool canPet;

    //Private Data
    DetectorSystem detector;
    public List<Pickable> nearPickables = new List<Pickable>();
    Pickable objectInHand = new();
    IController myController;
    bool isPetting;

    public void Initialize(DetectorSystem detector, IController controller)
    {
        this.detector = detector;
        myController = controller;
    }
    public void Update()
    {
        detector.CleanAllListsFromDestroyed();
        detector.CleanListsFromDestroyedObjects(nearPickables);
        RefreshPickables();
        UpdateHighlight();
        canShake = (detector.GetNear("Tree") != null) || (detector.GetNear("Rock") != null);

        if (objectInHand == null)
        {
            canPick = nearPickables.Count >= 1;
            canStore = false;
            canThrow = false;
            canPlant = false;
            gotSomethingInHand = false;
        }
        else
        {
            canStore = (nearPickables.Count >= 2) && (nearPickables[1].GetComponent<IStorableObject>() != null);
            canPick = false;
            canThrow = true;
            gotSomethingInHand = true;
        }

        if (objectInHand && objectInHand.GetComponent<Plantable>())
            canPlant = objectInHand.GetComponent<Plantable>().IsOnPlatingGround(this.myController.GetBody().transform.position);

        canPet = (detector.GetNear("NPC") != null) && (objectInHand == null) && (isPetting == false);
    }


    ////Interface
    public void PickNearestObject()
    {
        if ((nearPickables.Count > 0) ) 
        {
            float speed = (GetNearestPickable().GetSpeed());

            if (speed <= pickSpeedThrushold)
            {
                objectInHand = GetNearestPickable();
                PickObject(objectInHand);
            }
        }
    }
    public void PickObject(Pickable subject)
    {
        subject.Pick(this);

        canThrow = true;
        gotSomethingInHand = true;
    }
    public void PetNearestObject()
    {
        Transform petObject = nearPickables[0].transform;

        ConditionChecker condition = new ConditionChecker(true);

        ServicesProvider.instance.StartCoroutine(UpdatePetCondition(condition, petObject.gameObject.GetComponent<NPC>()));

        UIGame.instance.ShowRepeatingMessage("Petting", petObject, petTime, 5f, condition);

        if ((nearPickables.Count > 0))
        {
            if ((nearPickables[0].GetSpeed() <= pickSpeedThrushold))
            {
                NPC npc = (NPC)(nearPickables[0]);
                myController.GetBody().isKinematic = true;
                npc.StartPetting();
                isPetting = true;
                ServicesProvider.instance.StartCoroutine(PetObjectRoutine(condition,npc));
            }
        }
    }
    public void DropObjectInHand()
    {
        gotSomethingInHand = false;
        canThrow = false;

        if(objectInHand != null)
            objectInHand.Drop();

        objectInHand = null;
    }
    public void ThrowObjectInHand(Vector3 throwPoint)
    {
        if(objectInHand != null)
        {
            //Because Drop function removes the reference, so you have to hold a reference before dropping it.
            var objInHandTemp = objectInHand;

            DropObjectInHand();

            Vector3 direction = (throwPoint - objInHandTemp.transform.position).normalized;

            objInHandTemp.GetComponent<Rigidbody>().AddForce(direction * throwForce, ForceMode.Impulse);
        }
    }
    public void PlantObjectInHand()
    {
        Plantable platable = objectInHand.GetComponent<Plantable>();

        DropObjectInHand();

        Vector3 direction = (Vector3.down + (platable.plantDistance * myController.GetBody().transform.forward)).normalized;
        RaycastHit ray;
        Physics.Raycast(myHand.transform.position, direction, out ray, 50, myController.GetGroundDetector().GetGroundLayer());

        platable.Plant(ray.point);
    }
    public Pickable GetObjectInHand()
    {
        return objectInHand;
    }
    public void ClearObjectInHand()
    {
        objectInHand = null;
    }
    public void SetObjectInHand(Pickable obj)
    {
        objectInHand = obj;
    }
    public Pickable GetNearestPickable()
    {
        if (gotSomethingInHand && nearPickables.Count >= 2)
            return nearPickables[1];
        if (nearPickables.Count > 0)
            return nearPickables[0];
        else
            return null;
    }
    public Transform GetHand()
    {
        return myHand.transform;
    }


    //Pickables handle
    public void AddToPickable(IDetectable detectable)
    {
        if (detectable.GetGameObject().GetComponent<Pickable>())
        {
            foreach (string tag in pickableTags)
            {
                if (detectable.GetGameObject().tag == tag)
                {
                    if (nearPickables.Contains(detectable.GetGameObject().GetComponent<Pickable>()) == false)
                    {
                        nearPickables.Add(detectable.GetGameObject().GetComponent<Pickable>());
                    }
                }
            }
        }
    }
    public void RemoveFromPickables(IDetectable detectable)
    {
        if (detectable.GetGameObject().GetComponent<Pickable>())
        {
            foreach (string tag in pickableTags)
            {
                if (detectable.GetGameObject().tag == tag)
                {
                    if (nearPickables.Contains(detectable.GetGameObject().GetComponent<Pickable>()))
                    {
                        nearPickables.Remove(detectable.GetGameObject().GetComponent<Pickable>());

                        if (highlightToPick)
                            detectable.GetGameObject().GetComponent<Pickable>().PickablilityIndicator(false);
                    }
                }
            }
        }
    }

    public void RefreshPickables()
    {
        List<Pickable> toRemove = new List<Pickable>();

        foreach (Pickable detectedObject in nearPickables)
        {
            if (detector.GetNear().Contains(detectedObject) == false)
            {
                toRemove.Add(detectedObject);
            }
        }
        
        foreach (Pickable detectedObject in toRemove)
        {
            RemoveFromPickables(detectedObject);
        }

    }


    //Internal Algorithms
    void UpdateHighlight()
    {
        if (nearPickables.Count >= 1 && highlightToPick)
        {
            var nearest = GetNearestPickable();

            foreach (Pickable pickable in nearPickables)
            {
                if ((pickable == nearest) && (pickable != objectInHand) && (canPick || canStore))
                    pickable.PickablilityIndicator(true);
                else
                    pickable.PickablilityIndicator(false);
            }
        }
    }
    IEnumerator PetObjectRoutine(ConditionChecker condition, NPC npc)
    {
        while (condition.isTrue)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if(npc)
            npc.EndPetting();


        myController.GetBody().isKinematic = false;
        DropObjectInHand();

        isPetting = false;
    }
    IEnumerator UpdatePetCondition(ConditionChecker condition, NPC npc)
    {
        bool isConditionTrue = true;
        float time = 0;

        while (isConditionTrue)
        {
            condition.Update(true);

            isConditionTrue = ((time <= petTime) && (npc != null));

            time += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        condition.Update(false);
    }
}


