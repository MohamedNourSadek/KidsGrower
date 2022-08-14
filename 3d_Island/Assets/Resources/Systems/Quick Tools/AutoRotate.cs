using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] Vector3 defaultRotation = new Vector3();

    void FixedUpdate()
    {
        this.transform.rotation = Quaternion.Euler(defaultRotation.x
                                                 , defaultRotation.y + (speed * Time.realtimeSinceStartup),
                                                   defaultRotation.z);
    }
}
