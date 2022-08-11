using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundDetector
{
    [SerializeField] LayerMask _detectableLayers;
    [SerializeField] string _groundTag = "Ground";
    [SerializeField] string _waterTag = "Water";
    [SerializeField] float _onGroundThreshold = 1.3f;

    public static LayerMask _detectablelayers;
    
    public void Initialize()
    {
        _detectablelayers = _detectableLayers;
    }
    public static LayerMask GetGroundLayer()
    {
        return _detectablelayers;
    }
    public bool IsOnGroud(Rigidbody _body)
    {
        return DetectGround(_body, _groundTag);
    }
    public bool IsOnWater(Rigidbody _body)
    {
        return DetectGround(_body, _waterTag);
    }
    public void SetThreshold(float thrus)
    {
        _onGroundThreshold = thrus;
    }


    bool DetectGround(Rigidbody _body, string tag)
    {
        if (_body.isKinematic == false)
        {
            RaycastHit _ray;
            Physics.Raycast(_body.transform.position + Vector3.up, Vector2.down, out _ray, _onGroundThreshold, _detectableLayers);

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
