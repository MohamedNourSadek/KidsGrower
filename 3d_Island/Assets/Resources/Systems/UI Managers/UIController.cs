using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum PickMode { Pick, Drop, Shake};

public class UIController : MonoBehaviour, IPanelsManagerUser
{
    public static UIController instance;

    [SerializeField] List<PanelsManager> PanelsManagers;

    [Header("References")]
    [SerializeField] GameObject threeDCanvas;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject messagesArea;
    [SerializeField] GameObject highlightMessage;
    [SerializeField] GameObject ThreeDHighlightPrefab;
    [SerializeField] GameObject progressBarPrefab;
    [SerializeField] GameObject npcUiElementPrefab;
    [SerializeField] GameObject inventoryElementPrefab;
    [SerializeField] GameObject popUpMessage;

    [Header("Ui parameters")]

    [SerializeField] float buttonOnAlpha = 1f;
    [SerializeField] float buttonOffAlpha = 0.3f;


    [Header("UI Objects")]
    [SerializeField] Image pickDropButtonImage;
    [SerializeField] Image throwButtonImage;
    [SerializeField] Image plantButtonImage;
    [SerializeField] Image jumpButtonImage;
    [SerializeField] Image dashButtonImage;
    [SerializeField] Image petButtonImage;
    [SerializeField] Text pickDropButtonImage_Text;
    [SerializeField] Text customizeDebugger;
    [SerializeField] GameObject designMenus;
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] List<SliderElement> sliders = new List<SliderElement>();
    [SerializeField] public TextMeshProUGUI countDownText;

    [Header("Design Only")]
    [SerializeField] Text frameRateUi;

    Dictionary<GameObject, Slider> slidersContainer = new ();
    Dictionary<GameObject, UIElement_NPC> npcUiContainer = new();
    Dictionary<string, UiElement_Inventory> InventoryItemsContainer = new();

    //Helpers
    void Awake()
    {
        instance = this;

        foreach (SliderElement slider in sliders)
            slider.Initialize();

        foreach(PanelsManager panelManager in PanelsManagers)
            panelManager.Initialize();

    }
    void OnDrawGizmos()
    {
        foreach (PanelsManager panelManager in PanelsManagers)
            panelManager.OnDrawGizmos();
    }
    public List<SliderElement> GetSliders()
    {
        return sliders;
    }


    //UI control
    public void PickDropButton_Enable(bool _state)
    {
        ChangeAlpha(pickDropButtonImage, _state);
    }
    public void ThrowButton_Enable(bool _state)
    {
        ChangeAlpha(throwButtonImage, _state);
    }
    public void PlantButton_Enable(bool _state)
    {
        ChangeAlpha(plantButtonImage, _state);
    }
    public void JumpButton_Enable(bool _state)
    {
        ChangeAlpha(jumpButtonImage, _state);
    }
    public void DashButton_Enable(bool _state)
    {
        ChangeAlpha(dashButtonImage, _state);

    }
    public void PetButton_Enable(bool _state)
    {
        ChangeAlpha(petButtonImage, _state);
    }
    public void PickDropButton_SwitchMode(PickMode _mode)
    {
        pickDropButtonImage_Text.text = _mode.ToString();
    }
    public void UpdateFrameRate(string _frameRate)
    {
        frameRateUi.text = _frameRate;
    }


    //In Game UI
    public UIMessage PopUpMessage()
    {
        return Instantiate(popUpMessage, gameCanvas.transform).GetComponent<UIMessage>();
    }
    public void ShowUIMessage(string _message, float time, Vector3 startSize, float speed)
    {
        StartCoroutine(message(_message, time, startSize, speed));
    }
    public void RepeatInGameMessage(string _message, Transform _parent, float _messageTime, float _repeats, ConditionChecker _condition)
    {
        StartCoroutine(RepeatMessage_Coroutine(_message, _parent, _messageTime, _repeats, _condition));
    }
    public void CreateProgressBar(GameObject _user, Vector2 _limits, Transform _parent)
    {
        Slider _progressBar = Instantiate(progressBarPrefab, _parent.position, Quaternion.identity, threeDCanvas.transform).GetComponent<Slider>();
        _progressBar.minValue = _limits.x;
        _progressBar.maxValue = _limits.y;

        slidersContainer.Add(_user, _progressBar);

        StartCoroutine(TranslateUiElement(_progressBar.gameObject, _parent));
    }
    public void CreateNPCUi(GameObject _user, Vector2 _limits, Transform _parent)
    {
        UIElement_NPC _npcUi = Instantiate(npcUiElementPrefab, _parent.position, Quaternion.identity, threeDCanvas.transform).GetComponent<UIElement_NPC>();
       
        _npcUi.levelSlider.minValue = _limits.x;
        _npcUi.levelSlider.maxValue = _limits.y;

        npcUiContainer.Add(_user, _npcUi);

        StartCoroutine(TranslateUiElement(_npcUi.gameObject, _parent));
    }
    public void CreateInventoryUI(string _itemTag, UnityEngine.Events.UnityAction _onClick)
    {
        var _inventoryItem = Instantiate(inventoryElementPrefab, inGamePanel.transform).GetComponent<UiElement_Inventory>();

        _inventoryItem.elementButton.onClick.AddListener(_onClick);
        _inventoryItem.elementName.text = _itemTag;
        _inventoryItem.elementNo.text = 1.ToString();

        InventoryItemsContainer.Add(_itemTag, _inventoryItem);
    }
    public void UpdateProgressBar(GameObject _user, float _value)
    {
        slidersContainer[_user].value = _value;
    }
    public void UpdateProgressBarLimits(GameObject _user, Vector3 _limits)
    {
        slidersContainer[_user].minValue = _limits.x;
        slidersContainer[_user].maxValue = _limits.y;
    }
    public void UpateNpcUiElement(GameObject _user, float _value)
    {
        npcUiContainer[_user].levelSlider.value = _value;
    }
    public void UpateNpcUiElement(GameObject _user, string _text)
    {
        npcUiContainer[_user].levelText.text = _text;
    }
    public void UpateNpcUiElement(GameObject _user, Vector3 _limits)
    {
        npcUiContainer[_user].levelSlider.minValue = _limits.x;
        npcUiContainer[_user].levelSlider.maxValue = _limits.y;
    }
    public void UpdateInventoryUI(string _itemTag, int _nubmer)
    {
        InventoryItemsContainer[_itemTag].elementNo.text = _nubmer.ToString();
    }
    public void DestroyNpcUiElement(GameObject _user)
    {
        UIElement_NPC _temp = npcUiContainer[_user];
        npcUiContainer.Remove(_user);
        Destroy(_temp.gameObject);
    }
    public void DestroyProgressBar(GameObject _user)
    {
        Slider _temp = slidersContainer[_user];
        slidersContainer.Remove(_user);
        Destroy(_temp.gameObject);
    }
    public void DestroyInventoryUI(string _itemTag)
    {
        UiElement_Inventory _temp = InventoryItemsContainer[_itemTag];
        InventoryItemsContainer.Remove(_itemTag);
        Destroy(_temp.gameObject);
    }
    public void CustomizeLog(string text, Color color)
    {
        customizeDebugger.text = text;
        customizeDebugger.color = color;
    }
    public bool DoesInventoryItemExists(string _itemTag)
    {
        return (InventoryItemsContainer.ContainsKey(_itemTag));
    }


    //Control UI flow
    public void OpenMenuPanel(string _menuPanelName_PlusManagerNum)
    {
        PanelsManager.OpenMenuPanel(_menuPanelName_PlusManagerNum, PanelsManagers, true);
    }
    public void SetPanelDefault(string _menuPanelName_PlusManagerNum)
    {
        PanelsManager.SetDefault(_menuPanelName_PlusManagerNum, PanelsManagers);
    }
    public void OpenMenuPanelNonExclusive(string _menuPanelName_PlusManagerNum)
    {
        PanelsManager.OpenMenuPanel(_menuPanelName_PlusManagerNum, PanelsManagers, false);
    }
    public void CloseMenuPanelNonExclusive(string _menuPanelName_PlusManagerNum)
    {
        PanelsManager.CloseMenuPanel(_menuPanelName_PlusManagerNum, PanelsManagers);
    }
    public void CloseAllPanels()
    {
        foreach(PanelsManager panel in PanelsManagers)
        {
            panel.CloseAll();
        }
    }
    public void ToggleMenuPanel(string _menuInfo)
    {
        PanelsManager.TogglePanel(_menuInfo, PanelsManagers, false);
    }
    public void ShowIncrementPage(int _incrementTimesManagerNum)
    {
        int _managerNumber = Mathf.Abs(_incrementTimesManagerNum);
        
        int _increment = _incrementTimesManagerNum / Mathf.Abs(_incrementTimesManagerNum);

        string _targetPanel = PanelsManagers[_managerNumber].GetPanelRelativeToActive(_increment);

        var _directions = PanelsManagers[_managerNumber].GetPossibleDirection(PanelsManagers[_managerNumber].GetActivePage() + _increment);

        UpdateNextPrevious(_directions);

        PanelsManagers[_managerNumber].OpenMenuPanel(_targetPanel, true);
    }



    //Interal Algorithms
    IEnumerator message(string _message, float time, Vector3 startScale, float speed)
    {
        var messageObj = Instantiate(highlightMessage, messagesArea.transform);
        
        messageObj.transform.localScale = startScale;

        var Text = messageObj.GetComponent<TextMeshProUGUI>();

        Text.text =  _message;

        float _animationCurrentTime = time;

        while(_animationCurrentTime >= 0)
        {

            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, _animationCurrentTime / time);

            _animationCurrentTime -= Time.fixedDeltaTime;

            messageObj.transform.localScale += speed * (new Vector3(Time.fixedDeltaTime, Time.fixedDeltaTime, Time.fixedDeltaTime));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    void UpdateNextPrevious(ListPossibleDirections _directions)
    {
        previousButton.interactable = _directions.Previous;
        nextButton.interactable = _directions.Next;
    }
    IEnumerator TranslateUiElement(GameObject _object, Transform _parent)
    {
        while((_parent != null ) && (_object != null))
        {
            _object.transform.position = _parent.transform.position + Vector3.up;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
    }
    IEnumerator RepeatMessage_Coroutine(string _message, Transform _parent, float _messageTime, float _repeats, ConditionChecker _condition)
    {
        float _time = 0f;

        while (_condition.isTrue && _time<=_messageTime)
        {
            _time += _messageTime / _repeats;
            SpawnMessage(_message, (_parent.position + (1.5f*Vector3.up)));
            yield return new WaitForSecondsRealtime(_messageTime / _repeats);
        }
    }
    void SpawnMessage(string _Text, Vector3 _position)
    {
        var _gameObject = Instantiate(ThreeDHighlightPrefab, _position, Quaternion.identity, threeDCanvas.transform);
        _gameObject.GetComponentInChildren<Text>().text = _Text;
    }
    void ChangeAlpha(Image _myImage, bool _state)
    {
        _myImage.color =  new Color(_myImage.color.r, _myImage.color.g, _myImage.color.b, _state ? buttonOnAlpha : buttonOffAlpha);
    }
}


 