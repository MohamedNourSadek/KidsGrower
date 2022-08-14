using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideByCamera : MonoBehaviour
{
    public List<MeshRenderer>  meshesToHide = new List<MeshRenderer>();

    [SerializeField] float inCameraAlpha = 0.2f;
    [SerializeField] float normalAlpha = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            foreach (MeshRenderer mesh in meshesToHide)
            {
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, inCameraAlpha);
            }
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            foreach (MeshRenderer mesh in meshesToHide)
            {
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, normalAlpha);
            }
        }
    }
}
