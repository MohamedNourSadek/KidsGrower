using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundDetector
{
    [SerializeField] LayerMask detectableLayers;
    [SerializeField] string groundTag = "Ground";
    [SerializeField] string waterTag = "Water";
    [SerializeField] float onGroundThreshold = 1.3f;

    public static LayerMask detectablelayers;
    
    public void Initialize()
    {
        detectablelayers = detectableLayers;
    }
    public static LayerMask GetGroundLayer()
    {
        return detectablelayers;
    }
    public bool IsOnGroud(Rigidbody _body)
    {
        return DetectGround(_body, groundTag);
    }
    public bool IsOnWater(Rigidbody _body)
    {
        return DetectGround(_body, waterTag);
    }
    public void SetThreshold(float thrus)
    {
        onGroundThreshold = thrus;
    }

    bool DetectGround(Rigidbody _body, string tag)
    {
        if (_body.isKinematic == false)
        {
            RaycastHit _ray;
            Physics.Raycast(_body.transform.position + Vector3.up, Vector2.down, out _ray, onGroundThreshold, detectableLayers);

            if ((_ray.point.magnitude > 0) && (_ray.collider.tag == tag))
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }
}
