using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SliderElement
{
    [SerializeField] public string _saveName;
    [SerializeField] public Slider _mySlider;
    [SerializeField] public Text _valueText;

    public void Initialize()
    {
        _mySlider.onValueChanged.AddListener(OnValueChanged);
    }
    public void OnValueChanged(float value)
    {
        _valueText.text = _mySlider.value.ToString();
    }
}
