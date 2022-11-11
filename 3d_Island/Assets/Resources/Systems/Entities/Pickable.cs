using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickable : MonoBehaviour, IDetectable
{
    [SerializeField] Quaternion pickRotation;

    [Header("Objects References")]
    [SerializeField] protected GameObject indicatorObject;
    [SerializeField] protected Rigidbody myBody;
    [SerializeField] public GroundDetector groundDetector;
    [SerializeField] Collider myCollider;
    
    [System.NonSerialized] public HandSystem holder;
    [System.NonSerialized] public bool isPicked = false;

    int mylayer = 7;
    int mylayerNonDetectable = 0;

    public virtual void Awake()
    {
        groundDetector.Initialize(myBody);
    }
    void Update()
    {
        if (groundDetector.IsOnLayer(GroundLayers.Ground))
            gameObject.layer = mylayer;
        else 
            gameObject.layer = mylayerNonDetectable;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(SoundManager.instance != null && collision.relativeVelocity.magnitude >= 2f)
        {
            float factor = collision.relativeVelocity.magnitude / 20f;
            SoundManager.instance.PlayHit(this.gameObject, factor);
        }
    }


    //Interface
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
        myCollider.enabled = false;

        isPicked = true;
        myBody.isKinematic = true;
        myBody.velocity = Vector3.zero;

        this.transform.position = _picker.GetHand().position;
        this.transform.parent = _picker.GetHand();
        this.transform.localRotation = pickRotation;
    }
    public virtual void Drop()
    {
        isPicked = false;
        myBody.isKinematic = false;
        myCollider.enabled = true;

        if (holder != null)
            holder.ClearObjectInHand();

        holder = null;

        this.transform.parent = null;

    }
    public GameObject GetGameObject()
    {
        if (this.gameObject == null)    
            return null;
        else 
            return this.gameObject;
    }
    public Rigidbody GetBody()
    {
        if (myBody)
            return myBody;
        else
            return null;
    }
}
