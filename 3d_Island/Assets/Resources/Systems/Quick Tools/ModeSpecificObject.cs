using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSpecificObject : MonoBehaviour
{
    [SerializeField] GameObject FreeMode;
    [SerializeField] GameObject UnderPopulation;

    private void OnEnable()
    {
        if(DataManager.instance != null)
            if(DataManager.instance.GetCurrentMode() == modes.FreeMode)
            {
                FreeMode.SetActive(true);
                UnderPopulation.SetActive(false);
            }
            else if(DataManager.instance.GetCurrentMode() == modes.UnderPopulation)
            {
                FreeMode.SetActive(false);
                UnderPopulation.SetActive(true);
            }
    }

}
