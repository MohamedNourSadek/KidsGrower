using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] float _inCameraAlpha = 0.2f;
    [SerializeField] float _normalAlpha = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentsInChildren<MeshRenderer>().Length > 0)
        {
            var renderers = other.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer mesh in renderers)
            {
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, _inCameraAlpha);
            }
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponentsInChildren<MeshRenderer>().Length > 0)
        {
            var renderers = other.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer mesh in renderers)
            {
                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, _normalAlpha);
            }
        }
    }
}
