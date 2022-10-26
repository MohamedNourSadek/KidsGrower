using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float flyingHeight = 5f;
    [SerializeField] LeanTweenType animationType = LeanTweenType.easeInOutCubic;
    
    
    
    private void Start()
    {
        Explore();
    }

    void Explore()
    {
        Vector3 endPoint = MapSystem.instance.GetRandomExplorationPoint() + (flyingHeight * Vector3.up);
        
        float time = (endPoint - transform.position).magnitude;

        transform.LookAt(endPoint);

        int i = LeanTween.moveLocal(this.gameObject, endPoint, time / speed).setEase(animationType).id;

        LTDescr process = LeanTween.descr(i);

        process.setOnComplete(Explore);
    }
}
