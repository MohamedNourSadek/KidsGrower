using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PickableOjbects { NPC, Egg, Ball}

[System.Serializable]
public class HandSystem
{
    [Header("Pickable Parameters")]
    [SerializeField] GameObject _myHand;
    [SerializeField] List<PickableOjbects> _whoCanIPick;
    [SerializeField] bool _highLightPickable;
    [SerializeField] float _throwForce = 20f;
    [SerializeField] float _plantDistance = 1f;
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
    List<Pickable> _toPick = new();
    Pickable _objectInHand = new();
    float _nearObjectDistance;
    IHandController _myController;


    //Outside Interface
    public void Initialize(DetectorSystem detector, IHandController _controller)
    {
        _detector = detector;
        _nearObjectDistance = _detector._nearObjectDistance;
        _myController = _controller;
    }
    public void Update()
    { 
        try
        {
            if(_objectInHand == null)
            {
                if ((IsPickable(PickableOjbects.Ball)))
                {
                    Ball _ball = null;

                    if (_detector._ballDetectionStatus == BallDetectionStatus.VeryNear)
                         _ball = _detector.BallInRange(_nearObjectDistance);

                    foreach(Ball ball in _detector.GetDetectedData().balls)
                    {
                        if(ball == _ball)
                        {
                            if (!_toPick.Contains(ball))
                            {
                                if (_highLightPickable)
                                    ball.PickablilityIndicator(true);

                                _toPick.Add(ball);
                            }
                        }
                        else
                        {
                            if (_toPick.Contains(ball))
                            {
                                if (_highLightPickable)
                                    ball.PickablilityIndicator(false);

                                _toPick.Remove(ball);
                            }
                        }
                    }
                }
                if ((IsPickable(PickableOjbects.NPC)))
                {
                    NPC _npc = null;

                    if (_detector._npcDetectionStatus == NpcDetectionStatus.VeryNear)
                        _npc = _detector.NpcInRange(_nearObjectDistance);

                    foreach (NPC npc in _detector.GetDetectedData().npcs)
                    {
                        if (npc == _npc)
                        {
                            if (!_toPick.Contains(npc))
                            {
                                if (_highLightPickable)
                                    npc.PickablilityIndicator(true);

                                _toPick.Add(npc);
                            }

                        }
                        else
                        {
                            if (_toPick.Contains(npc))
                            {
                                if (_highLightPickable)
                                    npc.PickablilityIndicator(false);

                                _toPick.Remove(npc);
                            }
                        }
                    }
                }
                if ((IsPickable(PickableOjbects.Egg)))
                {
                    Egg _egg = null;

                    if (_detector._eggDetectionStatus == EggDetectionStatus.VeryNear)
                        _egg = _detector.EggInRange(_nearObjectDistance);

                    foreach (Egg egg in _detector.GetDetectedData().eggs)
                    {
                        if (egg == _egg)
                        {
                            if (!_toPick.Contains(egg))
                            {
                                if (_highLightPickable)
                                    egg.PickablilityIndicator(true);

                                _toPick.Add(egg);
                            }

                        }
                        else
                        {
                            if (_toPick.Contains(egg))
                            {
                                if (_highLightPickable)
                                    egg.PickablilityIndicator(false);

                                _toPick.Remove(egg);
                            }
                        }
                    }
                }


                if (_toPick.Count > 1)
                {
                    //Safty for destroyed Objects
                    List<Pickable> _newList = new();
                    foreach (Pickable pickable in _toPick)
                        if (pickable != null)
                            _newList.Add(pickable);

                    var temp = _newList[0];

                    foreach (Pickable pickable in _newList)
                        if (_detector.Distance(pickable.gameObject) < _detector.Distance(temp.gameObject))
                            temp = pickable;

                    foreach (Pickable pickable in _newList)
                        if (_highLightPickable)
                            pickable.PickablilityIndicator(false);

                    if (_highLightPickable)
                        temp.PickablilityIndicator(true);

                    _newList.Clear();
                    _newList.Add(temp);
                    _toPick = _newList;
                }



                _canPick = _toPick.Count > 0;
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
        }
        catch
        {

        }


        _canPet = (_detector._npcDetectionStatus == NpcDetectionStatus.VeryNear) && (_objectInHand == null);
    }
    public void PickObject()
    {
        if ((_toPick.Count > 0) ) 
        {
            if ((_toPick[0].GetSpeed() <= _pickSpeedThrushold))
            {
                _objectInHand = _toPick[0];

                _objectInHand.Pick(_myHand.transform);

                _canPick = false;
                _canDrop = true;
                _canThrow = true;
                _gotSomething = true;

                if (_objectInHand.GetType() == typeof(Egg))
                {
                    _canPlant = true;
                }
                else
                {
                    _canPlant = false;
                }
            }
        }
    }
    public void PetObject()
    {
        Vector3 _messagePosition = _detector.transform.position + (1f * Vector3.up);

        ConditionChecker condition = new ConditionChecker(true);
        _myController.StartCoroutine_Custom(UpdatePetCondition(condition));

        UIController.uIController.RepeatMessage("Petting", _messagePosition, _petTime, 5f, condition);

        if ((_toPick.Count > 0))
        {
            if ((_toPick[0].GetSpeed() <= _pickSpeedThrushold))
            {
                _objectInHand = _toPick[0];
                _myController.GetBody().isKinematic = true;
                ((NPC)_objectInHand).StartPetting();

                _canPick = false;
                _canDrop = true;
                _canThrow = true;
                _gotSomething = true;

                _myController.StartCoroutine_Custom(PetObjectRoutine(condition));
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
        Egg egg = (Egg)(_objectInHand);

        DropObject();

        ConditionChecker condition = new ConditionChecker(true);
        _myController.StartCoroutine_Custom(UpdateEggHatchCondition(condition, egg));
        UIController.uIController.ShowProgressBar(egg._hatchTime, egg.transform, condition);
        

        Vector3 _direction = (Vector3.down + _myController.GetBody().transform.forward).normalized;
        RaycastHit ray;
        Physics.Raycast(_myHand.transform.position, _direction, out ray, 50, GroundDetector.GetGroundLayer());
        egg.Plant(ray.point);
    }


    //Internal Algorithms
    bool IsPickable(PickableOjbects pickableType)
    {
        foreach(PickableOjbects _pickableType in _whoCanIPick)
        {
            if (_pickableType == pickableType)
                return true;
        }

        return false;
    }
    IEnumerator PetObjectRoutine(ConditionChecker condition)
    {
        while (condition.isTrue)
        {
            Debug.Log("Petting");

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        ((NPC)_objectInHand).EndPetting();
        _myController.GetBody().isKinematic = false;
        DropObject();
    }
    IEnumerator UpdateEggHatchCondition(ConditionChecker condition, Egg egg)
    {
        bool isConditionTrue = true;
        float _time = 0;

        while (isConditionTrue)
        {
            condition.Update(true);

            //0.95f to make the condition false before destroying the egg object.
            isConditionTrue = !egg.IsPicked() && (_time <= (0.95f * (egg._hatchTime)));

            _time += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        condition.Update(false);
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


public interface IHandController
{
    public Rigidbody GetBody();
    public void StartCoroutine_Custom(IEnumerator routine);
}