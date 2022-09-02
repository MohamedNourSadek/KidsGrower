using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DetectableElement
{
    [SerializeField] public string tag;
    [SerializeField] public DetectionStatus detectionStatus;
    [SerializeField] public bool notifyOnlyAtFirst;
    [SerializeField][Range(0, 50)] public int priority;

    public List<IDetectable> detectedList = new List<IDetectable>();
    public event notifyInRange OnInRange;
    public event notifyInRangeExit OnInRangeExit;
    public event notifyNear OnNear;

    public void InvokeOnRange(IDetectable detectable)
    {
        OnInRange?.Invoke(detectable);
    }
    public void InvokeOnRangeExit(IDetectable detectable)
    {
        OnInRangeExit?.Invoke(detectable);
    }
    public void InvokeNear(IDetectable detectable)
    {
        OnNear?.Invoke(detectable);
    }
}
