using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DetectionStatus { None, InRange, VeryNear };

public delegate void notifyInRange(IDetectable detectable);
public delegate void notifyInRangeExit(IDetectable detectable);
public delegate void notifyNear(IDetectable detectable);


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
        foreach(string pickableTag in whoCanIPick)
        {
            foreach(DetectableElement element in detectableElements)
            {
                if (pickableTag.ToString() == element.tag)
                {
                    IDetectable detectable = null;

                    if (element.detectionStatus == DetectionStatus.VeryNear)
                        detectable = DetectableNear(element.tag, nearObjectDistance);

                    foreach (IDetectable _detectable in element.detectedList)
                    {
                        if (_detectable == detectable)
                        {
                            if (!toPick.Contains((Pickable)_detectable))
                            {
                                if (highLightPickable)
                                    ((Pickable)_detectable).PickablilityIndicator(true);

                                toPick.Add((Pickable)_detectable);
                            }
                        }
                        else
                        {
                            if (toPick.Contains((Pickable)_detectable))
                            {
                                if (highLightPickable)
                                    ((Pickable)_detectable).PickablilityIndicator(false);

                                toPick.Remove((Pickable)_detectable);
                            }
                        }
                    }
                }


            }
        }

        if (toPick.Count > 1)
        {
            //Safty for destroyed Objects
            List<Pickable> newList = new();
            foreach (Pickable pickable in toPick)
                if (pickable != null)
                    newList.Add(pickable);

            var temp = newList[0];

            foreach (Pickable pickable in newList)
                if (Distance(pickable.gameObject) < Distance(temp.gameObject))
                    temp = pickable;

            foreach (Pickable pickable in newList)
                if (highLightPickable)
                    pickable.PickablilityIndicator(false);

            if (highLightPickable)
                temp.PickablilityIndicator(true);

            newList.Clear();
            newList.Add(temp);
            toPick = newList;
        }
    }
    public void UpdateStates()
    {
        foreach(DetectableElement element in detectableElements)
        {
            if (((MonoBehaviour)(DetectableNear(element.tag, nearObjectDistance))) != null)
            {
                if ((element.detectionStatus != DetectionStatus.VeryNear) || element.notifyOnlyAtFirst == false)
                    element.InvokeNear(DetectableNear(element.tag, nearObjectDistance));

                element.detectionStatus = DetectionStatus.VeryNear;
            }
            else if (((MonoBehaviour)(DetectableInRange(element.tag))) != null)
            {
                if (element.detectionStatus != DetectionStatus.InRange || element.notifyOnlyAtFirst == false)
                    element.InvokeInRange(DetectableInRange(element.tag));

                element.detectionStatus = DetectionStatus.InRange;
            }
            else
            {
                element.detectionStatus = DetectionStatus.None;
            }
        }
    }


    //Interfaces for outside use
    public List<Pickable> GetPickables()
    {
        return toPick;
    }
    public DetectableElement GetDetectable(string tag)
    {
        foreach(DetectableElement detectable in detectableElements)
            if (detectable.tag == tag)
                return detectable;

        return null; 
    }


    public IDetectable DetectableInRange(string tag)
    {
        IDetectable detectable = null;

        List<IDetectable> detectedList = GetDetectable(tag).detectedList;

        if (detectedList.Count == 1)
        {
            detectable = detectedList[0];
        }
        else if (detectedList.Count > 1)
        {
            detectable = detectedList[0];

            foreach (IDetectable _detectable in detectedList)
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
    public GameObject GetHighestProp(List<GameObject> list)
    {
        GameObject highest = list[0];

        foreach (GameObject obj in list)
            if (GetDetectable(obj.tag).priority > GetDetectable(highest.tag).priority)
                highest = obj;

        return highest;
    }

    float Distance(GameObject _object)
    {
        return (_object.transform.position - this.transform.position).magnitude;
    }
    bool IsNear(GameObject _object, float range)
    {
        return Distance(_object) <= range;
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
        foreach(DetectableElement element in detectableElements)
        {
            if(collider.CompareTag(element.tag))
            {
                IDetectable player = collider.GetComponentInParent<IDetectable>();

                if (element.detectedList.Contains(player) == false)
                    element.detectedList.Add(player);
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        foreach (DetectableElement element in detectableElements)
        {
            if (collider.CompareTag(element.tag))
            {
                IDetectable detectable = collider.GetComponentInParent<IDetectable>();

                if (element.detectedList.Contains(detectable) == true)
                {
                    element.detectedList.Remove(detectable);
                    element.InvokeInRangeExit(detectable);
                }
            }
        }
    }
}



