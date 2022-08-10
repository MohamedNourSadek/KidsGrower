using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundDetector
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] LayerMask _waterLayer;
    [SerializeField] float _onGroundThreshold = 1.3f;

    static LayerMask _globalGroundLayers;
    
    public void Initialize()
    {
        _globalGroundLayers = _groundLayer;
    }
    public static LayerMask GetGroundLayer()
    {
        return _globalGroundLayers;
    }
    public bool IsOnGroud(Rigidbody _body)
    {
        return DetectGround(_body, _groundLayer);
    }
    public bool IsOnWater(Rigidbody _body)
    {
        return DetectGround(_body, _waterLayer);
    }
    public void SetThreshold(float thrus)
    {
        _onGroundThreshold = thrus;
    }


    bool DetectGround(Rigidbody _body, LayerMask groundTag)
    {
        if (_body.isKinematic == false)
        {
            RaycastHit _ray;
            Physics.Raycast(_body.transform.position + Vector3.up, Vector2.down, out _ray, _onGroundThreshold, groundTag);
            return (_ray.point.magnitude > 0);
        }
        else
        {
            return false;
        }
    }
}
