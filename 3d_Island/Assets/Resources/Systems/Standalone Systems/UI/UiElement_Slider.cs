using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiElement_Slider : MonoBehaviour
{
    [SerializeField] Slider elementSlider;
    [SerializeField] Image fillImage;
   
    public void UpdateSliderValue(float newValue)
    {
        elementSlider.value = newValue;

        if(newValue >= (0.7 * elementSlider.maxValue))
        {
            fillImage.color = Color.green;
        }
        else if (newValue >= (.4 * elementSlider.maxValue))
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
    public void SetSliderRange(float maxValue)
    {
        elementSlider.maxValue = maxValue;
    }
}
