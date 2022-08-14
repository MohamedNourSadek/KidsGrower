using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DetectionStatus { None, InRange, VeryNear };

public delegate void notifyInRange(IDetectable _detectable);
public delegate void notifyInRangeExit(IDetectable _detectable);
public delegate void notifyNear(IDetectable _detectable);


public class DetectorSystem : MonoBehaviour
{
    [SerializeField] List<string> whoCanIPick;
    [SerializeField] bool highLightPickable;
    [SerializeField] public List<DetectableElement> detectableElements = new List<DetectableElement>();


    //Private data
    List<Pickable> toPick = new();
    float nearObjectDistance = 1f;

    public void Initialize(float _nearObjectDistance)
    {
        nearObjectDistance = _nearObjectDistance;
    }
    public void Update()
    {
        CleanAllListsFromDestroyed();
        
        UpdatePickables();
        UpdateStates();
    }

    public void CleanAllListsFromDestroyed()
    {
        CleanListsFromDestroyedObjects(toPick);

        foreach (DetectableElement element in detectableElements)
            CleanListsFromDestroyedObjects(element.detectedList);
    }
    public void UpdatePickables()
    {
        //Detecting near objects
        foreach(string _pickableTag in whoCanIPick)
        {
            foreach(DetectableElement _element in detectableElements)
            {
                if (_pickableTag.ToString() == _element.tag)
                {
                    IDetectable _detectable = null;

                    if (_element.detectionStatus == DetectionStatus.VeryNear)
                        _detectable = DetectableNear(_element.tag, nearObjectDistance);

                    foreach (IDetectable detectable in _element.detectedList)
                    {
                        if (detectable == _detectable)
                        {
                            if (!toPick.Contains((Pickable)detectable))
                            {
                                if (highLightPickable)
                                    ((Pickable)detectable).PickablilityIndicator(true);

                                toPick.Add((Pickable)detectable);
                            }
                        }
                        else
                        {
                            if (toPick.Contains((Pickable)detectable))
                            {
                                if (highLightPickable)
                                    ((Pickable)detectable).PickablilityIndicator(false);

                                toPick.Remove((Pickable)detectable);
                            }
                        }
                    }
                }


            }
        }

        if (toPick.Count > 1)
        {
            //Safty for destroyed Objects
            List<Pickable> _newList = new();
            foreach (Pickable _pickable in toPick)
                if (_pickable != null)
                    _newList.Add(_pickable);

            var _temp = _newList[0];

            foreach (Pickable _pickable in _newList)
                if (Distance(_pickable.gameObject) < Distance(_temp.gameObject))
                    _temp = _pickable;

            foreach (Pickable _pickable in _newList)
                if (highLightPickable)
                    _pickable.PickablilityIndicator(false);

            if (highLightPickable)
                _temp.PickablilityIndicator(true);

            _newList.Clear();
            _newList.Add(_temp);
            toPick = _newList;
        }
    }
    public void UpdateStates()
    {
        foreach(DetectableElement _element in detectableElements)
        {
            if (((MonoBehaviour)(DetectableNear(_element.tag, nearObjectDistance))) != null)
            {
                if ((_element.detectionStatus != DetectionStatus.VeryNear) || _element.notifyOnlyAtFirst == false)
                    _element.InvokeNear(DetectableNear(_element.tag, nearObjectDistance));

                _element.detectionStatus = DetectionStatus.VeryNear;
            }
            else if (((MonoBehaviour)(DetectableInRange(_element.tag))) != null)
            {
                if (_element.detectionStatus != DetectionStatus.InRange || _element.notifyOnlyAtFirst == false)
                    _element.InvokeOnRange(DetectableInRange(_element.tag));

                _element.detectionStatus = DetectionStatus.InRange;
            }
            else
            {
                _element.detectionStatus = DetectionStatus.None;
            }
        }
    }


    //Interfaces for outside use
    public List<Pickable> GetPickables()
    {
        return toPick;
    }
    public DetectableElement GetDetectable(string _tag)
    {
        foreach(DetectableElement _detectable in detectableElements)
            if (_detectable.tag == _tag)
                return _detectable;

        return null; 
    }


    public IDetectable DetectableInRange(string _tag)
    {
        IDetectable _detectable = null;

        List<IDetectable> _detectedList = GetDetectable(_tag).detectedList;

        if (_detectedList.Count == 1)
        {
            _detectable = _detectedList[0];
        }
        else if (_detectedList.Count > 1)
        {
            _detectable = _detectedList[0];

            foreach (IDetectable detectable in _detectedList)
                if (Distance(((MonoBehaviour)detectable).gameObject) < Distance(((MonoBehaviour)_detectable).gameObject))
                    _detectable = detectable;
        }

        return _detectable;
    }
    public IDetectable DetectableNear(string _tag, float _range)
    {
        IDetectable _detectable = DetectableInRange(_tag);

        if (_detectable != null && IsNear(((MonoBehaviour)_detectable).gameObject, _range))
        {
            return _detectable;
        }
        else
        {
            return null;
        }
    }


    //Help functions
    public GameObject GetHighestProp(List<GameObject> _list)
    {
        GameObject _highest = _list[0];

        foreach (GameObject _obj in _list)
            if (GetDetectable(_obj.tag).priority > GetDetectable(_highest.tag).priority)
                _highest = _obj;

        return _highest;
    }

    float Distance(GameObject _object)
    {
        return (_object.transform.position - this.transform.position).magnitude;
    }
    bool IsNear(GameObject _object, float _range)
    {
        return Distance(_object) <= _range;
    }
    void CleanListsFromDestroyedObjects(IList _list)
    {
        int _destroyedIndex = -1;
        for (int i = 0; i < _list.Count; i++)
        {
            if (((MonoBehaviour)_list[i]) == null)
                _destroyedIndex = i;
        }
        if (_destroyedIndex != -1)
            _list.RemoveAt(_destroyedIndex);
    }


    //Detection functions
    private void OnTriggerEnter(Collider _collider)
    {
        foreach(DetectableElement _element in detectableElements)
        {
            if(_collider.CompareTag(_element.tag))
            {
                IDetectable _player = _collider.GetComponentInParent<IDetectable>();

                if (_element.detectedList.Contains(_player) == false)
                    _element.detectedList.Add(_player);
            }
        }
    }
    private void OnTriggerExit(Collider _collider)
    {
        foreach (DetectableElement _element in detectableElements)
        {
            if (_collider.CompareTag(_element.tag))
            {
                IDetectable _detectable = _collider.GetComponentInParent<IDetectable>();

                if (_element.detectedList.Contains(_detectable) == true)
                {
                    _element.detectedList.Remove(_detectable);
                    _element.InvokeOnRangeExit(_detectable);
                }
            }
        }
    }
}



