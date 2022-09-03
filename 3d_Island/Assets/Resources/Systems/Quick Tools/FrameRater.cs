using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FrameRater : MonoBehaviour
{
    static int targetFrameRate = 60;
    TextMeshProUGUI frameRate;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        frameRate = GetComponent<TextMeshProUGUI>();
    }

    bool first = true;
    private void Update()
    {
        if((((int)(Time.timeSinceLevelLoad * 4f))%2) != 0 )
        {
            if (first == true)
            {
                first = false;
                frameRate.text = ((int)(1f / Time.deltaTime)).ToString();
            }
        }
        else
        {
            first = true;
        }
    }

}
