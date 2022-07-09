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
    [SerializeField] Image _pickDropButtonImage;
    [SerializeField] Image _throwButtonImage;
    [SerializeField] Image _plantButtonImage;
    [SerializeField] Image _jumpButtonImage;
    [SerializeField] Text _pickDropButtonImage_Text;

    public void PickDropButton_Enable(bool _state)
    {
        ChangeAlpha(_pickDropButtonImage, _state);
    }
    public void ThrowButton_Enable(bool _state)
    {
        ChangeAlpha(_throwButtonImage, _state);
    }
    public void PlantButton_Enable(bool _state)
    {
        ChangeAlpha(_plantButtonImage, _state);
    }
    public void JumpButton_Enable(bool _state)
    {
        ChangeAlpha(_jumpButtonImage, _state);
    }

    public void PickDropButton_SwitchMode(PickMode _mode)
    {
        _pickDropButtonImage_Text.text = _mode.ToString();
    }



    void ChangeAlpha(Image _myImage, bool _state)
    {
        _myImage.color =  new Color(_myImage.color.r, _myImage.color.g, _myImage.color.b, _state ? _buttonOnAlpha : _buttonOffAlpha);
    }
}
