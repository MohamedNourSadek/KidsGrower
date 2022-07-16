using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundDetector
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _onGroundThreshold = 1.3f;

    public bool IsOnGroud(Vector3 _position)
    {
        return DetectGround(_position);
    }
    bool DetectGround(Vector3 _position)
    {
        RaycastHit _ray;
        Physics.Raycast(_position + Vector3.up, Vector2.down, out _ray, _onGroundThreshold, _groundLayer);
        return (_ray.point.magnitude > 0);
    }
}
