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

    public DetectorSystem _detector;
    public bool _gotSomething;
    public bool _canPick;
    public bool _canDrop;
    public bool _canThrow;
    public bool _canPlant;

    //Private Data
    public List<Pickable> _toPick = new();
    public Pickable _objectInHand = new();
    float _nearObjectDistance;


    public void Initialize(DetectorSystem detector)
    {
        _detector = detector;
        _nearObjectDistance = _detector._nearObjectDistance;
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
        var _tempReference = (Egg)_objectInHand;
        DropObject();

        _tempReference.Plant(_myHand.transform.position + (_myHand.transform.forward * _plantDistance));
    }

    bool IsPickable(PickableOjbects pickableType)
    {
        foreach(PickableOjbects _pickableType in _whoCanIPick)
        {
            if (_pickableType == pickableType)
                return true;
        }

        return false;
    }
}
