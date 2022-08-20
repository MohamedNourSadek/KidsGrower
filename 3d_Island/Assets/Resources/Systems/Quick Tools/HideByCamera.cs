using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideByCamera : MonoBehaviour
{
    [SerializeField] GameObject visable;
    [SerializeField] GameObject hidden;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            visable.SetActive(false);
            hidden.SetActive(true);
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            visable.SetActive(true);
            hidden.SetActive(false);
        }
    }
}
