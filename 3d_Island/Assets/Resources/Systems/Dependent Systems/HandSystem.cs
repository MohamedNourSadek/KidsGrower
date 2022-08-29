using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class HandSystem
{
    [Header("Pickable Parameters")]
    [SerializeField] GameObject myHand;
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
    Pickable objectInHand = new();
    IController myController;
    bool isPetting;


    //Outside Interface
    public void Initialize(DetectorSystem _detector, IController _controller)
    {
        this.detector = _detector;
        myController = _controller;
    }
    public void Update()
    { 
        if(objectInHand == null)
        {
            canPick = detector.GetPickables().Count > 0;
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

        canPet = (detector.GetDetectable("NPC").detectionStatus == DetectionStatus.VeryNear) && (objectInHand == null) && (isPetting == false);
    }
    

    public void PickObject()
    {
        if ((detector.GetPickables().Count > 0) ) 
        {
            float _speed = (detector.GetPickables()[0].GetSpeed());

            if (_speed <= pickSpeedThrushold)
            {
                objectInHand = detector.GetPickables()[0];
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
        Transform _petObject = detector.GetPickables()[0].transform;

        ConditionChecker _condition = new ConditionChecker(true);

        ServicesProvider.instance.StartCoroutine(UpdatePetCondition(_condition));

        UIController.instance.RepeatInGameMessage("Petting", _petObject, petTime, 5f, _condition);

        if ((detector.GetPickables().Count > 0))
        {
            if ((detector.GetPickables()[0].GetSpeed() <= pickSpeedThrushold))
            {
                NPC _npc = (NPC)(detector.GetPickables()[0]);

                myController.GetBody().isKinematic = true;
                _npc.StartPetting();

                isPetting = true;

                canPick = false;

                ServicesProvider.instance.StartCoroutine(PetObjectRoutine(_condition,_npc));
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
    public void ThrowObject(Vector3 _target)
    {
        if(objectInHand != null)
        {
            //Because Drop function removes the reference
            var _tempReference = objectInHand;

            DropObject();

            Vector3 _direction = (_target - _tempReference.transform.position).normalized;

            _tempReference.GetComponent<Rigidbody>().AddForce(_direction * throwForce, ForceMode.Impulse);
        }
    }
    public void PlantObject()
    {
        Plantable _platable = objectInHand.GetComponent<Plantable>();

        DropObject();

        Vector3 _direction = (Vector3.down + (_platable.plantDistance * myController.GetBody().transform.forward)).normalized;
        RaycastHit _ray;
        Physics.Raycast(myHand.transform.position, _direction, out _ray, 50, GroundDetector.GetGroundLayer());

        _platable.Plant(_ray.point);
    }


    public Pickable GetObjectInHand()
    {
        return objectInHand;
    }
    public void ClearObjectInHand()
    {
        objectInHand = null;
    }
    public void SetObjectInHand(Pickable _obj)
    {
        objectInHand = _obj;
    }


    public Pickable GetNearest()
    {
        if (detector.GetPickables().Count > 0)
            return detector.GetPickables()[0];
        else
            return null;
    }
    public Transform GetHand()
    {
        return myHand.transform;
    }

     
    //Internal Algorithms
    IEnumerator PetObjectRoutine(ConditionChecker _condition, NPC _npc)
    {
        while (_condition.isTrue)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _npc.EndPetting();
        myController.GetBody().isKinematic = false;
        DropObject();

        isPetting = false;
    }
    IEnumerator UpdatePetCondition(ConditionChecker _condition)
    {
        bool _isConditionTrue = true;
        float _time = 0;

        while (_isConditionTrue)
        {
            _condition.Update(true);

            _isConditionTrue = (_time <= petTime);

            _time += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _condition.Update(false);
    }
}


