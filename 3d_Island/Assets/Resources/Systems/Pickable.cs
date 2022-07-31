using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickable : MonoBehaviour
{
    [Header("Objects References")]
    [SerializeField] protected GameObject _indicatorObject;
    [SerializeField] protected Rigidbody _myBody;
     
    protected bool _isPicked = false;

    public HandSystem _holder;

    public Rigidbody GetBody()
    {
        if (_myBody)
            return _myBody;
        else
            return null;
    }
    public float GetSpeed()
    {
        if (_myBody != null)
            return _myBody.velocity.magnitude;
        else
            return 0;
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
    public virtual void Pick(HandSystem _picker)
    {
        if (_holder != null)
            _holder._objectInHand = null;

        _holder = _picker;
        _holder._objectInHand = this;

        _isPicked = true;
        _myBody.isKinematic = true;
        _myBody.velocity = Vector3.zero;

        this.transform.position = _picker.GetHand().position;
        this.transform.parent = _picker.GetHand();
    }
    public virtual void Drop()
    {
        _isPicked = false;
        _myBody.isKinematic = false;

        _holder._objectInHand = null;
        _holder = null;

        this.transform.parent = null;

    }
}
