using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandSystem
{
    [Header("Pickable Parameters")]
    [SerializeField] bool _highLightPickable;
    [SerializeField] GameObject _myHand;
    [SerializeField] float _minDistanceToPick = 1f;
    [SerializeField] float _throwForce = 20f;
    [SerializeField] float _plantDistance = 1f;

    List<Pickable> _allPickables = new List<Pickable>();
    List<Pickable> nearPickables = new List<Pickable>();
    Pickable _objectInHand = new Pickable();

    public bool _gotSomething;
    public bool _canPick;
    public bool _canDrop;
    public bool _canThrow;
    public bool _canPlant;

    public void Initialize(List<Pickable> _pickables)
    {
        _allPickables = _pickables;
    }
    public void Update()
    {
        DetectInteractability();
    }


    void DetectInteractability()
    {
        //Remove Destroyed objects.
        foreach (var _pickable in nearPickables)
        {
            if (_pickable == null)
                nearPickables.Remove(_pickable);
        }

        //Nested inside a try catch in case the list changed while using it.
        try
        {
            if (_objectInHand == null)
            {
                foreach (Pickable _pickable in _allPickables)
                {
                    bool _isNear = false;
                    bool _alreadyExists = false;

                    if ((_pickable.transform.position - _myHand.transform.position).magnitude <= _minDistanceToPick)
                        _isNear = true;

                    if (nearPickables.Contains(_pickable))
                        _alreadyExists = true;

                    if (!_pickable.IsPicked() && _isNear && !_alreadyExists)
                    {
                        nearPickables.Add(_pickable);
                    }
                    else if (!_pickable.IsPicked() && !_isNear && _alreadyExists)
                    {
                        nearPickables.Remove(_pickable);
                    }

                    if(_highLightPickable)
                    {
                        if (!_pickable.IsPicked() && _isNear)
                            _pickable.PickablilityIndicator(true);
                        else
                            _pickable.PickablilityIndicator(false);
                    }
                }


                _canPick = nearPickables.Count > 0;
                _canThrow = false;
                _gotSomething = false;
                _canPlant = false;

            }
            else
            {

                if (_highLightPickable)
                {
                    foreach (Pickable _pickable in _allPickables)
                        _pickable.PickablilityIndicator(false);
                }

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
        _objectInHand = nearPickables[0];
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
    public void DropObject()
    {
        _canDrop = false;
        _canThrow = false;

        _objectInHand.Drop();
        _objectInHand = null;
    }
    public void ThrowObject(Vector3 direction)
    {
        //Because Drop function removes the reference
        var _tempReference = _objectInHand;

        DropObject();

        _tempReference.GetComponent<Rigidbody>().AddForce(direction * _throwForce, ForceMode.Impulse);
    }
    public void PlantObject()
    {
        var _tempReference = (Egg)_objectInHand;
        DropObject();

        _tempReference.Plant(_myHand.transform.position + (_myHand.transform.forward * _plantDistance));
    }
}
