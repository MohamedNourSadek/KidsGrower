using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public delegate void notifyInRange(IDetectable detectable);
public delegate void notifyInRangeExit(IDetectable detectable);
public delegate void notifyNear(IDetectable detectable);


public class DetectorSystem : MonoBehaviour
{
    [SerializeField] List<string> detectableTags = new List<string>();

    [SerializeField] List<IDetectable> detectableInRange = new List<IDetectable>();
    [SerializeField] List<IDetectable> detectableNear = new List<IDetectable>();

    event notifyInRange OnInRange;
    event notifyInRangeExit OnInRangeExit;
    event notifyNear OnNear;
    event notifyNear OnNearExit;

    //Private data
    float nearObjectDistance = 1f;

    public void Initialize(float _nearObjectDistance, notifyInRange inRange, notifyInRangeExit rangeExit, notifyNear near, notifyNear nearExit)
    {
        nearObjectDistance = _nearObjectDistance;
        OnInRange += inRange;
        OnInRangeExit += rangeExit;
        OnNear += near;
        OnNearExit += nearExit;
    }
    void Update()
    {
        CleanAllListsFromDestroyed();
    }


    //Interfaces for outside use
    public List<IDetectable> GetNear()
    {
        return detectableNear;
    }
    public List<IDetectable> GetInRange()
    {
        return detectableInRange;
    }
    public IDetectable GetInRange(string tag)
    {
        IDetectable detected = null;

        foreach(IDetectable detectable in detectableInRange)
        {
            if(detectable.tag == tag)
                detected = detectable;
        }

        return detected;
    }
    public IDetectable GetNear(string tag)
    {
        IDetectable detected = null;

        foreach (IDetectable detectable in detectableNear)
        {
            if (detectable.tag == tag)
                detected = detectable;
        }

        return detected;
    }


    //Notifications
    void OnTriggerEnter(Collider collider)
    {
        foreach(string detectableTag in detectableTags)
        {
            if(collider.CompareTag(detectableTag))
            {
                IDetectable detectedObject = collider.GetComponent<IDetectable>();

                if (detectedObject != null)
                {
                    if ((detectableInRange.Contains(detectedObject)) == false)
                    {
                        detectableInRange.Add(detectedObject);
                        OnInRange?.Invoke(detectedObject);
                    }
                }
            }
        }
    }
    void OnTriggerStay(Collider collider)
    {
        foreach (string detectableTag in detectableTags)
        {
            if (collider.CompareTag(detectableTag))
            {
                IDetectable detectedObject = collider.GetComponent<IDetectable>();

                if (detectedObject != null)
                {
                    //Case one: if it's near and added, do nothing.
                    //Case two: if it's near and not added, add it.
                    if ((detectableNear.Contains(detectedObject) == false) && (IsNear(detectedObject.GetGameObject()) == true))
                    {
                        detectableNear.Add(detectedObject);
                        OnNear?.Invoke(detectedObject);
                    }
                    //Case three: if it's not near, and added, remove it.
                    else if ((detectableNear.Contains(detectedObject) == true) && (IsNear(detectedObject.GetGameObject()) == false))
                    {
                        detectableNear.Remove(detectedObject);
                        OnNearExit?.Invoke(detectedObject);
                    }
                    //Case four: if it's not near and not added, do nothing.
                }
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        foreach (string detectableTag in detectableTags)
        {
            if (collider.CompareTag(detectableTag))
            {
                IDetectable detectedObject = collider.GetComponent<IDetectable>();

                if (detectableInRange.Contains(detectedObject))
                {
                    detectableInRange.Remove(detectedObject);
                    OnInRangeExit?.Invoke(detectedObject);
                }
            }
        }
    }


    //Internal Functions
    void CleanAllListsFromDestroyed()
    {
        CleanListsFromDestroyedObjects(detectableInRange);
        CleanListsFromDestroyedObjects(detectableNear);
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
    float Distance(GameObject _object)
    {
        return (_object.transform.position - this.transform.position).magnitude;
    }
    bool IsNear(GameObject _object)
    {
        return Distance(_object) <= nearObjectDistance;
    }
}



