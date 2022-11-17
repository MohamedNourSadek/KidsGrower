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


    public List<Pickable> nearPickables = new List<Pickable>();
    public bool isPetting;

    //Private Data
    AbilitySystem abilitySystem;
    DetectorSystem detector;
    Pickable objectInHand = new();
    IController myController;

    public void Initialize(DetectorSystem detector, IController controller, AbilitySystem abilitySystem)
    {
        this.detector = detector;
        myController = controller;
        this.abilitySystem = abilitySystem; 
    }
    public void Update()
    {
        detector.CleanAllListsFromDestroyed();
        detector.CleanListsFromDestroyedObjects(nearPickables);
        RefreshPickables();
        UpdateHighlight();
        ReprioritizePickablesOnDistance();
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
        if (objectInHand != null)
        {
            AddToPickable(objectInHand);
            objectInHand.Drop();
        }

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
        if(nearPickables.Count >= 1)
        {
            return nearPickables[0];
        }
        else
        {
            return null;
        }
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
            if ((detector.GetNear().Contains(detectedObject) == false) || detectedObject.isPicked)
                toRemove.Add(detectedObject);

        }
        
        foreach (Pickable detectedObject in toRemove)
        {
            RemoveFromPickables(detectedObject);
        }

    }


    //Internal Algorithms
    void ReprioritizePickablesOnDistance()
    {
        if(nearPickables.Count > 0)
        {
            Pickable nearest = nearPickables[0];

            foreach (Pickable p in nearPickables)
            {
                if(Distance(p.gameObject) < Distance(nearest.gameObject))
                {
                    nearest= p;
                }
            }

            if (nearest != nearPickables[0])
            {
                Pickable temp = nearPickables[0];
                nearPickables.Remove(temp);
                nearPickables[0] = nearest;
                nearPickables.Add(temp);
            }
        }

    }
    void UpdateHighlight()
    {
        if (nearPickables.Count >= 1 && highlightToPick)
        {
            var nearestPickable = GetNearestPickable();

            foreach (Pickable pickable in nearPickables)
            {
                if ((pickable == nearestPickable) && (abilitySystem.canPick || abilitySystem.canStore))
                    pickable.PickablilityIndicator(true);
                else
                    pickable.PickablilityIndicator(false);
            }
        }
    }
    float Distance(GameObject obj)
    {
        return (this.myHand.transform.position - obj.transform.position).magnitude;
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


