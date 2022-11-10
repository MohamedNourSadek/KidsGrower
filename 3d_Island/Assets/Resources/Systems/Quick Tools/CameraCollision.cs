using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] float factor;
    
    Vector3 modification;
    Vector3 point;
    Vector3 distance;

    public Vector3 GetCollisionModification()
    {
        return modification;
    }

    void Update()
    {
        RaycastHit hit;
        
        Physics.SphereCast(this.gameObject.transform.position, 0f , Vector3.down, out hit);

        if (hit.collider && (hit.collider.tag == "Ground" || hit.collider.tag == "Rock"))
        {
            point = hit.point;
            distance = (point - transform.position);
            modification = factor * (1f/distance.magnitude) * distance.normalized;
        }
        else
        {
            point = Vector3.zero;
            modification = Vector3.zero;
        }
    }
}
