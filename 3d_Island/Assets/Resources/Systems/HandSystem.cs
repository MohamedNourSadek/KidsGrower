using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class HandSystem
{
    [Header("Pickable Parameters")]
    [SerializeField] GameObject _myHand;
    [SerializeField] float _throwForce = 20f;
    [SerializeField] float _pickSpeedThrushold = 2f;
    [SerializeField] float _petTime = 1f;
     
    public DetectorSystem _detector;
    public bool _gotSomething;
    public bool _canPick;
    public bool _canDrop;
    public bool _canThrow;
    public bool _canPlant; 
    public bool _canPet;
     

    //Private Data
    public Pickable _objectInHand = new();
    float _nearObjectDistance;
    IController _myController;


    //Outside Interface
    public void Initialize(DetectorSystem detector, IController _controller)
    {
        _detector = detector;
        _nearObjectDistance = _detector._nearObjectDistance;
        _myController = _controller;
    }
    public void Update()
    { 
        if(_objectInHand == null)
        {
            _canPick = _detector.GetPickables().Count > 0;
            _canThrow = false;
            _gotSomething = false;
            _canPlant = false;
        }
        else
        {
            _canPick = false;
            _canDrop = true;
            _canThrow = true;
            _gotSomething = true;
        }

        if (_objectInHand && _objectInHand.GetComponent<Plantable>())
            _canPlant = _objectInHand.GetComponent<Plantable>().IsOnPlatingGround(this._myController.GetBody().transform.position);

        _canPet = (_detector.GetDetectable("NPC")._detectionStatus == DetectionStatus.VeryNear) && (_objectInHand == null);
    }
    

    public void PickObject()
    {
        if ((_detector.GetPickables().Count > 0) ) 
        {
            float _speed = (_detector.GetPickables()[0].GetSpeed());

            if (_speed <= _pickSpeedThrushold)
            {
                _objectInHand = _detector.GetPickables()[0];
                _objectInHand.Pick(this);

                _canPick = false;
                _canDrop = true;
                _canThrow = true;
                _gotSomething = true;
            }
        }
    }
    public void PetObject()
    {
        Transform _petObject = _detector.GetPickables()[0].transform;

        ConditionChecker condition = new ConditionChecker(true);
        CoRoutineProvider.instance.StartCoroutine(UpdatePetCondition(condition));

        UIController.instance.RepeatMessage("Petting", _petObject, _petTime, 5f, condition);

        if ((_detector.GetPickables().Count > 0))
        {
            if ((_detector.GetPickables()[0].GetSpeed() <= _pickSpeedThrushold))
            {
                NPC _npc = (NPC)(_detector.GetPickables()[0]);

                _myController.GetBody().isKinematic = true;
                _npc.StartPetting();

                _canPick = false;

                CoRoutineProvider.instance.StartCoroutine(PetObjectRoutine(condition,_npc));
            }
        }
    }
    public void DropObject()
    {
        _canDrop = false;
        _canThrow = false;

        if(_objectInHand != null)
            _objectInHand.Drop();

        _objectInHand = null;
    }
    public void ThrowObject(Vector3 target)
    {
        if(_objectInHand != null)
        {
            //Because Drop function removes the reference
            var _tempReference = _objectInHand;

            DropObject();

            Vector3 _direction = (target - _tempReference.transform.position).normalized;

            _tempReference.GetComponent<Rigidbody>().AddForce(_direction * _throwForce, ForceMode.Impulse);
        }
    }
    public void PlantObject()
    {
        Plantable _platable = _objectInHand.GetComponent<Plantable>();

        DropObject();

        Vector3 _direction = (Vector3.down + (_platable._plantDistance * _myController.GetBody().transform.forward)).normalized;
        RaycastHit ray;
        Physics.Raycast(_myHand.transform.position, _direction, out ray, 50, GroundDetector.GetGroundLayer());

        _platable.Plant(ray.point);
    }
    public Pickable ObjectInHand()
    {
        return _objectInHand;
    }
    public Pickable GetNearest()
    {
        if (_detector.GetPickables().Count > 0)
            return _detector.GetPickables()[0];
        else
            return null;
    }
    public Transform GetHand()
    {
        return _myHand.transform;
    }

     
    //Internal Algorithms
    IEnumerator PetObjectRoutine(ConditionChecker condition, NPC _npc)
    {
        while (condition.isTrue)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _npc.EndPetting();
        _myController.GetBody().isKinematic = false;
        DropObject();
    }
    IEnumerator UpdatePetCondition(ConditionChecker condition)
    {
        bool isConditionTrue = true;
        float _time = 0;

        while (isConditionTrue)
        {
            condition.Update(true);

            isConditionTrue = (_time <= _petTime);

            _time += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        condition.Update(false);
    }
}


public interface IController
{
    public Rigidbody GetBody();
}