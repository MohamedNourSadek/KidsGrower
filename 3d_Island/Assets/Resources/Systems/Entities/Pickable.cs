using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickable : MonoBehaviour, IDetectable
{
    [Header("Objects References")]
    [SerializeField] protected GameObject indicatorObject;
    [SerializeField] protected Rigidbody myBody;

    public HandSystem holder;
    protected bool isPicked = false;

    public Rigidbody GetBody()
    {
        if (myBody)
            return myBody;
        else
            return null;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(SoundManager.instance != null && collision.relativeVelocity.magnitude >= 2f)
        {
            float factor = collision.relativeVelocity.magnitude / 20f;
            SoundManager.instance.PlayHit(this.gameObject, factor);
        }
    }
    public float GetSpeed()
    {
        if (myBody != null)
            return myBody.velocity.magnitude;
        else
            return 0;
    }
    public bool IsPicked()
    {
        return isPicked;
    }
    public virtual void PickablilityIndicator(bool _status)
    {
        if(_status)
        {
            indicatorObject.SetActive(true);
        }
        else
        {
            indicatorObject.SetActive(false);
        }
    }
    public virtual void Pick(HandSystem _picker)
    {
        if (holder != null)
            holder.ClearObjectInHand();

        holder = _picker;
        holder.SetObjectInHand(this);

        isPicked = true;
        myBody.isKinematic = true;
        myBody.velocity = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        this.transform.position = _picker.GetHand().position;
        this.transform.parent = _picker.GetHand();
    }
    public virtual void Drop()
    {
        isPicked = false;
        myBody.isKinematic = false;

        holder.ClearObjectInHand();
        holder = null;

        this.transform.parent = null;

    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
