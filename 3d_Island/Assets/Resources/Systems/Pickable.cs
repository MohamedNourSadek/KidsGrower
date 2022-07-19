using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickable : MonoBehaviour
{
    [Header("Objects References")]
    [SerializeField] protected GameObject _indicatorObject;
    [SerializeField] protected Rigidbody _myBody;
     
    protected bool _isPicked = false;

    public float GetSpeed()
    {
        return _myBody.velocity.magnitude;
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
        _isPicked = true;
        _myBody.isKinematic = true;
        _myBody.velocity = Vector3.zero;

        this.transform.position = handPosition.transform.position;
        this.transform.parent = handPosition;
    }
    public virtual void Drop()
    {
        _isPicked = false;
        _myBody.isKinematic = false;

        this.transform.parent = null;

    }
}
