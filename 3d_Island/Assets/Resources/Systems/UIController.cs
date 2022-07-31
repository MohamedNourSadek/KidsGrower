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
    [SerializeField] GameObject _page1Menu;
    [SerializeField] GameObject _page2Menu;
    [SerializeField] List<SliderElement> _sliders = new List<SliderElement>();

    [Header("Design Only")]
    [SerializeField] Text _frameRateUi;

    Dictionary<GameObject, Slider> _slidersContainer = new Dictionary<GameObject, Slider>();

    //Helpers
    void Awake()
    {
        uIController = this;

        foreach (SliderElement slider in _sliders)
            slider.Initialize();
    }
    public List<SliderElement> GetSliders()
    {
        return _sliders;
    }


    //UI control
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
    public void UpdateFrameRate(string frameRate)
    {
        _frameRateUi.text = frameRate;
    }


    //In Game UI
    public void RepeatMessage(string message, Transform parent, float messageTime, float repeats, ConditionChecker condition)
    {
        StartCoroutine(RepeatMessage_Coroutine(message, parent, messageTime, repeats, condition));
    }
    public void CreateProgressBar(GameObject _user, Vector2 limits, Transform parent)
    {
        Slider _progressBar = Instantiate(_progressBarPrefab, parent.position, Quaternion.identity, _3dCanvas.transform).GetComponent<Slider>();
        _progressBar.minValue = limits.x;
        _progressBar.maxValue = limits.y;

        _slidersContainer.Add(_user, _progressBar);

        StartCoroutine(TranslateProgressBar(_progressBar.gameObject, parent));
    }
    public void UpdateProgressBar(GameObject _user, float _value)
    {
        _slidersContainer[_user].value = _value;
    }
    public void UpdateProgressBarLimits(GameObject _user, Vector3 _limits)
    {
        _slidersContainer[_user].minValue = _limits.x;
        _slidersContainer[_user].maxValue = _limits.y;
    }
    public void DestroyProgressBar(GameObject _user)
    {
        Slider _temp = _slidersContainer[_user];
        _slidersContainer.Remove(_user);
        Destroy(_temp.gameObject);
    }
    
    
    //Control UI flow
    public void ShowSettings(bool state)
    {
        _allSettingsMenus.SetActive(state);
        _allGameMenus.SetActive(!state);
    }
    public void ShowSettings_Page1()
    {
        _page1Menu.SetActive(true);
        _page2Menu.SetActive(false);
    }
    public void ShowSettings_Page2()
    {
        _page1Menu.SetActive(false);
        _page2Menu.SetActive(true);
    }

    
    
    //Interal Algorithms
    IEnumerator TranslateProgressBar(GameObject _object, Transform parent)
    {
        while((parent != null ) && (_object != null))
        {
            _object.transform.position = parent.transform.position + Vector3.up;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
    }
    IEnumerator RepeatMessage_Coroutine(string message, Transform parent, float messageTime, float repeats, ConditionChecker condition)
    {
        float _time = 0f;

        while (condition.isTrue && _time<=messageTime)
        {
            _time += messageTime / repeats;
            SpawnMessage(message, (parent.position + (1.5f*Vector3.up)));
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



 