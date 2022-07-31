using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideByCamera : MonoBehaviour
{
    public List<MeshRenderer>  _meshesToHide = new List<MeshRenderer>();

    [SerializeField] float _inCameraAlpha = 0.2f;
    [SerializeField] float _normalAlpha = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            foreach (MeshRenderer mesh in _meshesToHide)
            {
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, _inCameraAlpha);
            }
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            foreach (MeshRenderer mesh in _meshesToHide)
            {
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, _normalAlpha);
            }
        }
    }
}
