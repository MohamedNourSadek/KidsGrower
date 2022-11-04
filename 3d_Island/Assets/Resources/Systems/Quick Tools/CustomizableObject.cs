using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableObject : MonoBehaviour
{
    [SerializeField] GameObject normalObject;
    [SerializeField] GameObject selectedObject;

    public void SetSelectState(bool state)
    {
        if(state)
        {
            normalObject.SetActive(false);
            selectedObject.SetActive(true);
        }
        else
        {
            normalObject.SetActive(true);
            selectedObject.SetActive(false);
        }
    }

}
