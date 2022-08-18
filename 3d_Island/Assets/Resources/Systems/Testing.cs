using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Testing : MonoBehaviour
{
    public GameObject gm;

    void Update()
    {
        if(Input.GetKeyDown("x"))
        {
            var objects = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

            objects.UpdateModules();
        }

        
    }
}
