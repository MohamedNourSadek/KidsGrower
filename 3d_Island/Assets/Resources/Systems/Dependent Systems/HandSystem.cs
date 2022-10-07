using System.Collections;
using System.Collections.Generic;
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
     
    public DetectorSystem detector;
    public bool gotSomething;
    public bool canPick;
    public bool canDrop;
    public bool canThrow;
    public bool canPlant; 
    public bool canPet;

    //Private Data
    [SerializeField] List<Pickable> nearPickables = new List<Pickable>();
    Pickable objectInHand = new();
    IController myController;
    bool isPetting;

    //Outside Interface
    public void Initialize(DetectorSystem detector, IController controller)
    {
        this.detector = detector;
        myController = controller;
    }
    public void Update()
    {
        UpdatePickables();

        if (objectInHand == null)
        {
            canPick = GetPickables().Count > 0;
            canThrow = false;
            gotSomething = false;
            canPlant = false;
        }
        else
        {
            canPick = false;
            canDrop = true;
            canThrow = true;
            gotSomething = true;
        }

        if (objectInHand && objectInHand.GetComponent<Plantable>())
            canPlant = objectInHand.GetComponent<Plantable>().IsOnPlatingGround(this.myController.GetBody().transform.position);

        canPet = (detector.GetNear("NPC") != null) && (objectInHand == null) && (isPetting == false);
    }
    public List<Pickable> GetPickables()
    {
        return nearPickables;
    }

    public void PickObject()
    {
        if ((GetPickables().Count > 0) ) 
        {
            float speed = (GetPickables()[0].GetSpeed());

            if (speed <= pickSpeedThrushold)
            {
                objectInHand = GetPickables()[0];
                objectInHand.Pick(this);

                canPick = false;
                canDrop = true;
                canThrow = true;
                gotSomething = true;
            }
        }
    }
    public void PetObject()
    {
        Transform petObject = GetPickables()[0].transform;

        ConditionChecker condition = new ConditionChecker(true);

        ServicesProvider.instance.StartCoroutine(UpdatePetCondition(condition, petObject.gameObject.GetComponent<NPC>()));

        UIGame.instance.ShowRepeatingMessage("Petting", petObject, petTime, 5f, condition);

        if ((GetPickables().Count > 0))
        {
            if ((GetPickables()[0].GetSpeed() <= pickSpeedThrushold))
            {
                NPC npc = (NPC)(GetPickables()[0]);

                myController.GetBody().isKinematic = true;
                npc.StartPetting();

                isPetting = true;

                canPick = false;

                ServicesProvider.instance.StartCoroutine(PetObjectRoutine(condition,npc));
            }
        }
    }
    public void DropObject()
    {
        canDrop = false;
        canThrow = false;

        if(objectInHand != null)
            objectInHand.Drop();

        objectInHand = null;
    }
    public void ThrowObject(Vector3 target)
    {
        if(objectInHand != null)
        {
            //Because Drop function removes the reference
            var tempReference = objectInHand;

            DropObject();

            Vector3 direction = (target - tempReference.transform.position).normalized;

            tempReference.GetComponent<Rigidbody>().AddForce(direction * throwForce, ForceMode.Impulse);
        }
    }
    public void PlantObject()
    {
        Plantable platable = objectInHand.GetComponent<Plantable>();

        DropObject();

        Vector3 direction = (Vector3.down + (platable.plantDistance * myController.GetBody().transform.forward)).normalized;
        RaycastHit ray;
        Physics.Raycast(myHand.transform.position, direction, out ray, 50, GroundDetector.GetGroundLayer());

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


    public Pickable GetNearest()
    {
        if (GetPickables().Count > 0)
            return GetPickables()[0];
        else
            return null;
    }
    public Transform GetHand()
    {
        return myHand.transform;
    }

     
    //Internal Algorithms
    IEnumerator PetObjectRoutine(ConditionChecker condition, NPC npc)
    {
        while (condition.isTrue)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if(npc)
            npc.EndPetting();


        myController.GetBody().isKinematic = false;
        DropObject();

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
    
    void UpdatePickables()
    {
        nearPickables.Clear();

        foreach(IDetectable detectable in detector.GetInRange())
        {
            foreach (string tag in pickableTags)
            {
                if (detectable.GetGameObject().tag == tag && highlightToPick)
                {
                    detectable.GetGameObject().GetComponent<Pickable>().PickablilityIndicator(false);
                }
            }
        }

        foreach(IDetectable detectable in  detector.GetNear())
        {
            bool targetTag = false;

            foreach(string tag in pickableTags)
            {
                if(detectable.GetGameObject().tag == tag)
                {
                    targetTag = true;
                }
            }

            if(targetTag)
            {
                nearPickables.Add(detectable.GetGameObject().GetComponent<Pickable>());
            }
        }

        if(nearPickables.Count >= 1 && highlightToPick)
            GetNearest().PickablilityIndicator(true);
    }
}


