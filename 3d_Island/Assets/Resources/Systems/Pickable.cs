using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void notify();

public class Pickable : MonoBehaviour
{
    [Header("Objects References")]
    [SerializeField] GameObject _indicatorObject;
    [SerializeField] Rigidbody _myBody;
     
    bool isPicked = false;


    public bool IsPicked()
    {
        return isPicked;
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

        isPicked = true;
        _myBody.isKinematic = true;

        this.transform.position = handPosition.transform.position;
        this.transform.parent = handPosition;
    }

    public virtual void Drop()
    {
        PickablilityIndicator(true);

        isPicked = false;
        _myBody.isKinematic = false;

        this.transform.parent = null;

    }
}
