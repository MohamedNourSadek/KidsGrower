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

    [Header("Design Only")]
    [SerializeField] NPC _npcAsset;
    [SerializeField] Text _frameRate;
    [SerializeField] Slider _growingUpTime;
    [SerializeField] Text _growingUpTimeText;
    [SerializeField] Slider _timeToGetBored;
    [SerializeField] Text _timeToGetBoredText;
    [SerializeField] Slider _sleepTime;
    [SerializeField] Text _sleepTimeText;
    [SerializeField] Slider _seekPlayer;
    [SerializeField] Text _seekPlayerText;
    [SerializeField] Slider _seekNPC;
    [SerializeField] Text _seekNPCText;
    [SerializeField] Slider _seekTree;
    [SerializeField] Text _seekTreeText;
    [SerializeField] Slider _seekBall;
    [SerializeField] Text _seekBallText;
    [SerializeField] Slider _dropBall;
    [SerializeField] Text _dropBallText;
    [SerializeField] Slider _throwBallOnNPC;
    [SerializeField] Text _throwBallOnNPCText;
    [SerializeField] Slider _throwBallOnPlayer;
    [SerializeField] Text _throwBallOnPlayerText;
    [SerializeField] Slider _punchNPC;
    [SerializeField] Text _punchNPCText;


    private void Awake()
    {
        uIController = this;

        _growingUpTime.onValueChanged.AddListener(OnGrowingUpTime_Changed);
        OnGrowingUpTime_Changed(_growingUpTime.value);

        _timeToGetBored.onValueChanged.AddListener(OnTimeToGetBored_Changed);
        OnTimeToGetBored_Changed(_timeToGetBored.value);

        _sleepTime.onValueChanged.AddListener(OnSleepTime_Changed);
        OnSleepTime_Changed(_sleepTime.value);

        _seekPlayer.onValueChanged.AddListener(OnSeekPlayer_Changed);
        OnSeekPlayer_Changed(_seekPlayer.value);

        _seekNPC.onValueChanged.AddListener(OnSeekNPC_Changed);
        OnSeekNPC_Changed(_seekNPC.value);

        _seekTree.onValueChanged.AddListener(OnSeekTree_Changed);
        OnSeekTree_Changed(_seekTree.value);

        _seekBall.onValueChanged.AddListener(OnSeekBall_Changed);
        OnSeekBall_Changed(_seekBall.value);

        _dropBall.onValueChanged.AddListener(OnDropBallChanged);
        OnDropBallChanged(_dropBall.value);

        _throwBallOnNPC.onValueChanged.AddListener(OnThrowBallOnNPC_Changed);
        OnThrowBallOnNPC_Changed(_throwBallOnNPC.value);

        _throwBallOnPlayer.onValueChanged.AddListener(OnThrowBallOnPlayer_Changed);
        OnThrowBallOnPlayer_Changed(_throwBallOnPlayer.value);

        _punchNPC.onValueChanged.AddListener(OnPunchNPC_Changed);
        OnPunchNPC_Changed(_punchNPC.value);
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
    public void ApplySettings()
    {
        var _npcs = FindObjectsOfType<NPC>();
        List<NPC> npcList = new List<NPC>();
        foreach (NPC npc in _npcs)
        {
            npcList.Add(npc);
        }
        npcList.Add(_npcAsset);


        foreach (NPC npc in npcList)
        {
            npc.growingUpTime = _growingUpTime.value;
            npc._bordemTime = _timeToGetBored.value;
            npc._sleepTime = _sleepTime.value;
            npc._playerLove = _seekPlayer.value;
            npc._npcLove = _seekNPC.value;
            npc._ballLove = _seekBall.value;
            npc._treeLove = _seekTree.value;
            npc._droppingBall = _dropBall.value;
            npc._throwBallOnNPC = _throwBallOnNPC.value;
            npc._throwBallOnPlayer = _throwBallOnPlayer.value;
            npc._punchNpcLove = _punchNPC.value;
        }    


        ShowSettings(false);
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

    
    //Events
    public void OnGrowingUpTime_Changed(float newValue)
    {
        _growingUpTimeText.text = newValue.ToString() + " Seconds";
    }
    public void OnTimeToGetBored_Changed(float newValue)
    {
        _timeToGetBoredText.text = newValue.ToString() + " Seconds";
    }
    public void OnSleepTime_Changed(float newValue)
    {
        _sleepTimeText.text = (newValue).ToString() + " Seconds";
    }
    public void OnSeekPlayer_Changed(float newValue)
    {
        _seekPlayerText.text = (newValue * 100).ToString() + " %";
    }
    public void OnSeekNPC_Changed(float newValue)
    {
        _seekNPCText.text = (newValue * 100).ToString() + " %";
    }
    public void OnSeekTree_Changed(float newValue)
    {
        _seekTreeText.text = (newValue * 100).ToString() + " %";
    }
    public void OnSeekBall_Changed(float newValue)
    {
        _seekBallText.text = (newValue * 100).ToString() + " %";
    }
    public void OnDropBallChanged(float newValue)
    {
        _dropBallText.text = (newValue * 100).ToString() + " %";
    }
    public void OnThrowBallOnNPC_Changed(float newValue)
    {
        _throwBallOnNPCText.text = (newValue * 100).ToString() + " %";
    }
    public void OnThrowBallOnPlayer_Changed(float newValue)
    {
        _throwBallOnPlayerText.text = (newValue * 100).ToString() + " %";
    }
    public void OnPunchNPC_Changed(float newValue)
    {
        _punchNPCText.text = (newValue * 100).ToString() + " %";
    }

}
