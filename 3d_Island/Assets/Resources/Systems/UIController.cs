using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum PickMode { Pick, Drop, Shake};

public class UIController : MonoBehaviour
{

    public static UIController uIController;

    [Header("References")]
    [SerializeField] GameObject _3dCanvas;
    [SerializeField] GameObject _3dHighlightPrefab;
    [SerializeField] GameObject _progressBarPrefab;

    [Header("Ui parameters")]
    [SerializeField] float _buttonOnAlpha = 1f;
    [SerializeField] float _buttonOffAlpha = 0.3f;


    [Header("UI Objects")]
    [SerializeField] Image _pickDropButtonImage;
    [SerializeField] Image _throwButtonImage;
    [SerializeField] Image _plantButtonImage;
    [SerializeField] Image _jumpButtonImage;
    [SerializeField] Image _dashButtonImage;
    [SerializeField] Image _petButtonImage;
    [SerializeField] Text _pickDropButtonImage_Text;
    [SerializeField] GameObject _allSettingsMenus;
    [SerializeField] GameObject _allGameMenus;
    [SerializeField] List<SliderElement> _sliders = new List<SliderElement>();


    [Header("Design Only")]
    [SerializeField] Text _frameRate;


    private void Awake()
    {
        uIController = this;

        foreach (SliderElement slider in _sliders)
            slider.Initialize();
    }
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
    public void DashButton_Enable(bool _state)
    {
        ChangeAlpha(_dashButtonImage, _state);

    }
    public void PetButton_Enable(bool _state)
    {
        ChangeAlpha(_petButtonImage, _state);
    }
    public void PickDropButton_SwitchMode(PickMode _mode)
    {
        _pickDropButtonImage_Text.text = _mode.ToString();
    }

    public void RepeatMessage(string message, Vector3 position, float messageTime, float repeats, ConditionChecker condition)
    {
        StartCoroutine(RepeatMessage_Coroutine(message, position, messageTime, repeats, condition));
    }
    public void ShowProgressBar(float max, Transform parent, ConditionChecker condition)
    {
        Slider _progressBar = Instantiate(_progressBarPrefab, parent.position, Quaternion.identity, _3dCanvas.transform).GetComponent<Slider>();
       
        //0.95f of the real max to make the slider finish first
        _progressBar.maxValue = 0.95f * max;

        StartCoroutine(ProgressBar(parent, condition, _progressBar));
    }
    public void ShowFrameRate(string frameRate)
    {
        _frameRate.text = frameRate;
    }
    public void ShowSettings(bool state)
    {
        _allSettingsMenus.SetActive(state);
        _allGameMenus.SetActive(!state);
    }
    public List<SliderElement> GetSliders()
    {
        return _sliders;
    }

    
    
    //Interal Algorithms
    IEnumerator ProgressBar(Transform parent, ConditionChecker condition, Slider progressBar)
    {
        while(condition.isTrue)
        {
            progressBar.value += Time.fixedDeltaTime;
            progressBar.gameObject.transform.position = parent.transform.position + Vector3.up;

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        Destroy(progressBar.gameObject);
    }
    IEnumerator RepeatMessage_Coroutine(string message, Vector3 position, float messageTime, float repeats, ConditionChecker condition)
    {
        float _time = 0f;

        while (condition.isTrue)
        {
            _time += messageTime / repeats;
            SpawnMessage(message, position);
            yield return new WaitForSecondsRealtime(messageTime / repeats);
        }
    }
    void SpawnMessage(string Text, Vector3 position)
    {
        var GameObject = Instantiate(_3dHighlightPrefab, position, Quaternion.identity, _3dCanvas.transform);
        GameObject.GetComponentInChildren<Text>().text = Text;
    }
    void ChangeAlpha(Image _myImage, bool _state)
    {
        _myImage.color =  new Color(_myImage.color.r, _myImage.color.g, _myImage.color.b, _state ? _buttonOnAlpha : _buttonOffAlpha);
    }
}

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
