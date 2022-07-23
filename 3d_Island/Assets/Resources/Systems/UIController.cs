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


    private void Awake()
    {
        uIController = this;
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
    public void RepeatMessage(string message, Vector3 position, float messageTime, float repeats)
    {
        StartCoroutine(RepeatMessage_Coroutine(message, position, messageTime, repeats));
    }
    public void ShowProgressBar(float max, Transform parent, Pickable pickable)
    {
        Slider _progressBar = Instantiate(_progressBarPrefab, parent.position, Quaternion.identity, _3dCanvas.transform).GetComponent<Slider>();
       //0.95f of the real max to make the slider finish first
        _progressBar.maxValue = 0.95f * max;

        StartCoroutine(ProgressBar(parent, pickable, _progressBar));
    }

    IEnumerator ProgressBar(Transform parent, Pickable pickable, Slider progressBar)
    {
        while((pickable.IsPicked() == false) && (progressBar.value < (progressBar.maxValue)))
        {
            progressBar.value += Time.fixedDeltaTime;
            progressBar.gameObject.transform.position = parent.transform.position + Vector3.up;

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);

        }

        Destroy(progressBar.gameObject);
    }
    IEnumerator RepeatMessage_Coroutine(string message, Vector3 position, float messageTime, float repeats)
    {
        float _time = 0f;

        while (_time <= messageTime)
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
