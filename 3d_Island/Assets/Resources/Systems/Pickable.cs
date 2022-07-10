using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void notify();

public class Pickable : MonoBehaviour
{
    public static List<Pickable> _allPickables = new List<Pickable>();

    
    [Header("Objects References")]
    [SerializeField] protected GameObject _indicatorObject;
    [SerializeField] protected Rigidbody _myBody;
     
    protected bool _isPicked = false;

    public virtual void Awake()
    {
        _allPickables.Add(this);
    }
    void OnDestroy()
    {
        _allPickables.Remove(this);
    }


    public bool IsPicked()
    {
        return _isPicked;
    }
    public virtual void PickablilityIndicator(bool _status)
    {
        if(_status)
        {
            _indicatorObject.SetActive(true);
        }
        else
        {
            _indicatorObject.SetActive(false);
        }
    }
    public virtual void Pick(Transform handPosition)
    {
        PickablilityIndicator(false);

        _isPicked = true;
        _myBody.isKinematic = true;
        _myBody.velocity = Vector3.zero;

        this.transform.position = handPosition.transform.position;
        this.transform.parent = handPosition;
    }
    public virtual void Drop()
    {
        PickablilityIndicator(true);

        _isPicked = false;
        _myBody.isKinematic = false;

        this.transform.parent = null;

    }
}
