using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SliderElement
{
    [SerializeField] public string saveName;
    [SerializeField] public Slider mySlider;
    [SerializeField] public Text valueText;


    public void Initialize()
    {
        mySlider.onValueChanged.AddListener(OnValueChanged);
    }
    public void OnValueChanged(float value)
    {
        valueText.text = mySlider.value.ToString();
    }
}
