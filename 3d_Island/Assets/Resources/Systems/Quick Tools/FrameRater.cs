using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FrameRater : MonoBehaviour
{
    static int targetFrameRate = 1000;

    TextMeshProUGUI frameRate;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;

        frameRate = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Coroutine());
    }
    
    IEnumerator Coroutine()
    {
        while (true)
        {
            yield return null;
            frameRate.text = ((int)(1f / Time.deltaTime)).ToString();
        }
    }

}
