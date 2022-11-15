using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ServicesProvider : MonoBehaviour
{
    [SerializeField] GameObject boxCaster;

    public static ServicesProvider instance;

    List<Action> OnGizomsEvent = new List<Action>();

    public void SubScribe_OnGizoms(Action function)
    {
        OnGizomsEvent.Add(function);
    }
    void Awake()
    {
        instance = this;       
    }
    void OnDrawGizmos()
    {
        foreach(Action action in OnGizomsEvent)
        {
            action.Invoke();
        }

    }
    public void DestroyObject(GameObject gameobject)
    {
        Destroy(gameobject);
    }
    public BoxCaster CreateBoxCaster(Vector3 origin)
    {
        return Instantiate(boxCaster,origin, Quaternion.identity).GetComponent<BoxCaster>();   
    }
}
