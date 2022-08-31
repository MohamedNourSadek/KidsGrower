using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AIParameterSlider : AIParameter
{
    [SerializeField] public Slider mySlider;
    [SerializeField] public Text valueText;

    public void Initialize()
    {
        mySlider.onValueChanged.AddListener(OnValueChanged);
    }
    public void OnValueChanged(float _value)
    {
        value = mySlider.value;
        valueText.text = mySlider.value.ToString();
    }
    public void ChangeSlider(float _value)
    {
        mySlider.value = _value;
    }

}
