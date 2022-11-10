using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GroundDetector
{
    [SerializeField] LayerMask detectableLayers;
    [SerializeField] float onGroundThreshold = 1.3f;

    Rigidbody myBody;
    public bool initialized;

    public void Initialize(Rigidbody body)
    {
        myBody = body;
        initialized = true;
    }


    //Interface
    public bool IsOnLayer(GroundLayers layerTag)
    {
        if (initialized)
        {
            if (myBody.isKinematic == false)
            {
                RaycastHit ray;
                Physics.Raycast(myBody.transform.position + Vector3.up, Vector2.down, out ray, onGroundThreshold, detectableLayers);

                if ((ray.point.magnitude > 0) && (ray.collider.tag == layerTag.ToString()))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }
    public float GetDistanceFromGround()
    {
        RaycastHit ray;

        Physics.Raycast(myBody.transform.position + Vector3.up, Vector2.down, out ray, 50f, detectableLayers);

        if (ray.collider.tag == "Ground")
        {
            return (myBody.transform.position - ray.point).magnitude;
        }
        else
        {
            return -1f;
        }
    }
    public LayerMask GetGroundLayer()
    {
        return detectableLayers;
    }
}


public enum GroundLayers { Ground, Water }