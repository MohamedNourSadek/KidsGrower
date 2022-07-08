using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum PickMode { Pick, Drop};

public class UIController : MonoBehaviour
{
    [Header("Ui parameters")]
    [SerializeField] float _buttonOnAlpha = 1f;
    [SerializeField] float _buttonOffAlpha = 0.3f;


    [Header("UI Objects")]
    [SerializeField] Image _pickButtonImage;
    [SerializeField] Image _throwButtonImage;
    [SerializeField] Text _pickButtonText;

    public void PickButton_Enable(bool _state)
    {
        if (_state)
            _pickButtonImage.color = new Color(_pickButtonImage.color.r, _pickButtonImage.color.g, _pickButtonImage.color.b, 1f);
        else
            _pickButtonImage.color = new Color(_pickButtonImage.color.r, _pickButtonImage.color.g, _pickButtonImage.color.b, 0.3f);
    }
    public void PickButton_SwitchMode(PickMode _mode)
    {
        _pickButtonText.text = _mode.ToString();
    }

    public void ThrowButton_Enable(bool _state)
    {
        if (_state)
            _throwButtonImage.color = new Color(_throwButtonImage.color.r, _throwButtonImage.color.g, _throwButtonImage.color.b, _buttonOnAlpha);
        else
            _throwButtonImage.color = new Color(_throwButtonImage.color.r, _throwButtonImage.color.g, _throwButtonImage.color.b, _buttonOffAlpha);
    }

}
