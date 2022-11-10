using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    [SerializeField] PostProcessingFunctions postProcessingFunctions;

    public static MenuManager instance;

    private void Awake()
    {
        instance = this;

        postProcessingFunctions.Initialize();
        postProcessingFunctions.SetBlur(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
