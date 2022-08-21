using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] float factor;
    
    Vector3 modification;
    Vector3 _point;
    Vector3 _distance;

    public Vector3 GetCollisionModification()
    {
        return modification;
    }

    void Update()
    {
        RaycastHit _hit;
        
        Physics.SphereCast(this.gameObject.transform.position, 0f , Vector3.down, out _hit);

        if (_hit.collider && (_hit.collider.tag == "Ground" || _hit.collider.tag == "Rock"))
        {
            _point = _hit.point;
            _distance = (_point - transform.position);
            modification = factor * (1f/_distance.magnitude) * _distance.normalized;
        }
        else
        {
            _point = Vector3.zero;
            modification = Vector3.zero;
        }
    }
}
