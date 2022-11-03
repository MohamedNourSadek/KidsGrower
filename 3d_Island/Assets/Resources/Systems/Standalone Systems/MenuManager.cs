using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    [SerializeField] PostProcessingFunctions postProcessingFunctions;

    private void Awake()
    {
        postProcessingFunctions.Initialize();
        postProcessingFunctions.SetBlur(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
