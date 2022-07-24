using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundDetector
{
    [SerializeField] LayerMask _groundLayer;
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
        return DetectGround(_body);
    }


    bool DetectGround(Rigidbody _body)
    {
        if (_body.isKinematic == false)
        {
            RaycastHit _ray;
            Physics.Raycast(_body.transform.position + Vector3.up, Vector2.down, out _ray, _onGroundThreshold, _groundLayer);
            return (_ray.point.magnitude > 0);
        }
        else
        {
            return false;
        }
    }
}
