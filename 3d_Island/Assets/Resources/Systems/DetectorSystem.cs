using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum PickableTags { NPC, Egg, Ball, Fruit, Seed, Harvest }

public class DetectorSystem : MonoBehaviour
{
    [SerializeField] List<PickableTags> _whoCanIPick;
    [SerializeField] bool _highLightPickable;
    [SerializeField] public List<DetectableElement> _detectableElements = new List<DetectableElement>();

    public float _nearObjectDistance = 1f;

    //Private data
    public List<Pickable> _toPick = new();

    public void Initialize(float nearObjectDistance)
    {
        _nearObjectDistance = nearObjectDistance;
    }
    public void Update()
    {
        CleanAllListsFromDestroyed();
        
        UpdatePickables();
        UpdateStates();
    }

    public void CleanAllListsFromDestroyed()
    {
        CleanListsFromDestroyedObjects(_toPick);

        foreach (DetectableElement element in _detectableElements)
            CleanListsFromDestroyedObjects(element._detectedList);
    }
    public void UpdatePickables()
    {
        //Detecting near objects
        foreach(PickableTags pickableTag in _whoCanIPick)
        {
            foreach(DetectableElement element in _detectableElements)
            {

                if (pickableTag.ToString() == element.Tag)
                {
                    IDetectable _detectable = null;

                    if (element._detectionStatus == DetectionStatus.VeryNear)
                        _detectable = DetectableNear(element.Tag, _nearObjectDistance);

                    foreach (IDetectable detectable in element._detectedList)
                    {
                        if (detectable == _detectable)
                        {
                            if (!_toPick.Contains((Pickable)detectable))
                            {
                                if (_highLightPickable)
                                    ((Pickable)detectable).PickablilityIndicator(true);

                                _toPick.Add((Pickable)detectable);
                            }
                        }
                        else
                        {
                            if (_toPick.Contains((Pickable)detectable))
                            {
                                if (_highLightPickable)
                                    ((Pickable)detectable).PickablilityIndicator(false);

                                _toPick.Remove((Pickable)detectable);
                            }
                        }
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
                if (Distance(pickable.gameObject) < Distance(temp.gameObject))
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
    }
    public void UpdateStates()
    {
        foreach(DetectableElement element in _detectableElements)
        {
            if (((MonoBehaviour)(DetectableNear(element.Tag, _nearObjectDistance))) != null)
            {
                if ((element._detectionStatus != DetectionStatus.VeryNear) || element._notifyOnlyAtFirst == false)
                    element.InvokeNear(DetectableNear(element.Tag, _nearObjectDistance));

                element._detectionStatus = DetectionStatus.VeryNear;
            }
            else if (((MonoBehaviour)(DetectableInRange(element.Tag))) != null)
            {
                if (element._detectionStatus != DetectionStatus.InRange || element._notifyOnlyAtFirst == false)
                    element.InvokeOnRange(DetectableInRange(element.Tag));

                element._detectionStatus = DetectionStatus.InRange;
            }
            else
            {
                element._detectionStatus = DetectionStatus.None;
            }
        }
    }


    //Interfaces for outside use
    public List<Pickable> GetPickables()
    {
        return _toPick;
    }
    public DetectableElement GetDetectable(string tag)
    {
        foreach(DetectableElement detectable in _detectableElements)
            if (detectable.Tag == tag)
                return detectable;

        return null; 
    }


    public IDetectable DetectableInRange(string tag)
    {
        IDetectable detectable = null;

        List<IDetectable> _detectedList = GetDetectable(tag)._detectedList;

        if (_detectedList.Count == 1)
        {
            detectable = _detectedList[0];
        }
        else if (_detectedList.Count > 1)
        {
            detectable = _detectedList[0];

            foreach (IDetectable _detectable in _detectedList)
                if (Distance(((MonoBehaviour)_detectable).gameObject) < Distance(((MonoBehaviour)detectable).gameObject))
                    detectable = _detectable;
        }

        return detectable;
    }
    public IDetectable DetectableNear(string tag, float range)
    {
        IDetectable detectable = DetectableInRange(tag);

        if (detectable != null && IsNear(((MonoBehaviour)detectable).gameObject, range))
        {
            return detectable;
        }
        else
        {
            return null;
        }
    }


    //Help functions
    public GameObject GetHighestProp(List<GameObject> _list)
    {
        GameObject highest = _list[0];

        foreach (GameObject _obj in _list)
            if (GetDetectable(_obj.tag)._priority > GetDetectable(highest.tag)._priority)
                highest = _obj;

        return highest;
    }

    float Distance(GameObject _object)
    {
        return (_object.transform.position - this.transform.position).magnitude;
    }
    bool IsNear(GameObject _object, float _range)
    {
        return Distance(_object) <= _range;
    }
    void CleanListsFromDestroyedObjects(IList list)
    {
        int destroyedIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (((MonoBehaviour)list[i]) == null)
                destroyedIndex = i;
        }
        if (destroyedIndex != -1)
            list.RemoveAt(destroyedIndex);
    }


    //Detection functions
    private void OnTriggerEnter(Collider collider)
    {
        foreach(DetectableElement element in _detectableElements)
        {
            if(collider.CompareTag(element.Tag))
            {
                IDetectable _player = collider.GetComponentInParent<IDetectable>();

                if (element._detectedList.Contains(_player) == false)
                    element._detectedList.Add(_player);
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        foreach (DetectableElement element in _detectableElements)
        {
            if (collider.CompareTag(element.Tag))
            {
                IDetectable _detectable = collider.GetComponentInParent<IDetectable>();

                if (element._detectedList.Contains(_detectable) == true)
                {
                    element._detectedList.Remove(_detectable);
                    element.InvokeOnRangeExit(_detectable);
                }
            }
        }
    }
}

public enum DetectionStatus { None, InRange, VeryNear };

public delegate void notifyInRange(IDetectable _detectable);
public delegate void notifyInRangeExit(IDetectable _detectable);
public delegate void notifyNear(IDetectable _detectable);


[System.Serializable]
public class DetectableElement
{
    [SerializeField] public string Tag;
    [SerializeField] public DetectionStatus _detectionStatus;
    [SerializeField] public bool _notifyOnlyAtFirst;
    [SerializeField] [Range(0,50)] public int _priority;

    public List<IDetectable> _detectedList = new List<IDetectable>();
    public event notifyInRange _OnInRange;
    public event notifyInRangeExit _OnInRangeExit;
    public event notifyNear _OnNear;

    public void InvokeOnRange(IDetectable _detectable)
    {
        _OnInRange?.Invoke(_detectable);
    }
    public void InvokeOnRangeExit(IDetectable _detectable)
    {
        _OnInRangeExit?.Invoke(_detectable);
    }
    public void InvokeNear(IDetectable _detectable)
    {
        _OnNear?.Invoke(_detectable);
    }
}
public interface IDetectable
{
    public string tag
    {
        get;
        set;
    }
    public GameObject GetGameObject();
}
