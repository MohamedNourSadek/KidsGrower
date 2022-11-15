using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCaster : MonoBehaviour
{
    List<GameObject> objects = new List<GameObject>();

    public List<GameObject> GetObjectsInRange()
    {
        return objects;
    }
    private void OnTriggerEnter(Collider other)
    {
        objects.Add(other.gameObject);
    }
}
