using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public enum ButtonMode { Pick, Drop, Shake, Eat, Tear, Store,Attack, Dress, Plant, _};

public class UIGame : MonoBehaviour
{
    public static UIGame instance;

    [SerializeField] List<PanelsManager> PanelsManagers;

    [Header("References")]
    [SerializeField] public GameObject threeDCanvas;
    [SerializeField] public GameObject gameCanvas;
    [SerializeField] GameObject declarationsArea;
    [SerializeField] GameObject inGamePanel;
    [SerializeField] GameObject messagesArea;
    [SerializeField] GameObject highlightMessage;
    [SerializeField] GameObject ThreeDHighlightPrefab;
    [SerializeField] GameObject progressBarPrefab;
    [SerializeField] GameObject npcUiElementPrefab;
    [SerializeField] GameObject silderElementPrefab;
    [SerializeField] GameObject popUpMessageAsset;
    [SerializeField] GameObject touchControls;
    [SerializeField] GameObject itemsParent;
    [SerializeField] GameObject inventoryElementUIAsset;

    [Header("UI parameters")]

    [SerializeField] float buttonOnAlpha = 1f;
    [SerializeField] float buttonOffAlpha = 0.3f;

    [Header("UI Objects")]
    [SerializeField] NPCStatsUI npcStatsUI;
    [SerializeField] ListAttributeUI characterAttributesUI;
    [SerializeField] GameObject npcStatsEdit;
    [SerializeField] TextMeshProUGUI npcStatsName;
    [SerializeField] Image pickDropButtonImage;
    [SerializeField] Image throwButtonImage;
    [SerializeField] Image attackButtonImage;
    [SerializeField] Image storeButtonImage;
    [SerializeField] Image plantButtonImage;
    [SerializeField] Image jumpButtonImage;
    [SerializeField] Image dashButtonImage;
    [SerializeField] Image petButtonImage;
    [SerializeField] Text pickDropButtonImage_Text;
    [SerializeField] Text attackButtonButtonImage_Text;
    [SerializeField] Text plantButtonButtonImage_Text;
    [SerializeField] Text customizeDebugger;
    [SerializeField] GameObject designMenus;
    [SerializeField] public TextMeshProUGUI countDownText;
    [SerializeField] GameObject savingText;
    [SerializeField] GameObject delcareUIAsset;


    //Helpers
    void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            touchControls.SetActive(false);

        instance = this;

        foreach(PanelsManager panelManager in PanelsManagers)
            panelManager.Initialize();
    }
    void OnDrawGizmos()
    {
        foreach (PanelsManager panelManager in PanelsManagers)
            panelManager.OnDrawGizmos();
    }


    //UI control
    public void PickDropButton_Enable(bool _state)
    {
        ChangeAlpha(pickDropButtonImage, _state);
    }
    public void AttackTearButton_Enable(bool _state)
    {
        ChangeAlpha(attackButtonImage, _state);
    }
    public void StoreButton_Enable(bool _state)
    {
        ChangeAlpha(storeButtonImage, _state);
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
    public void PickDropButton_SwitchMode(ButtonMode _mode)
    {
        pickDropButtonImage_Text.text = _mode.ToString();
    }
    public void PlantEatButton_SwitchMode(ButtonMode _mode)
    {
        plantButtonButtonImage_Text.text = _mode.ToString();
    }
    public void AttackTearButton_SwitchMode(ButtonMode _mode)
    {
        attackButtonButtonImage_Text.text = _mode.ToString();
    }
    public void ChangeCustomizingIndicator(string text, Color color)
    {
        customizeDebugger.text = text;
        customizeDebugger.color = color;
    }
    public NPCStatsUI GetNPCStatsUI()
    {
        return npcStatsUI;
    }
    public void EditNPCStats(bool state)
    {
        npcStatsEdit.SetActive(state);
    }
    public string GetUiName()
    {
        return npcStatsName.text;
    }
    public void UpdateFullNPCStats(CharacterParameters data)
    {
        characterAttributesUI.UpdateUI(data);
    }



    // In Game UI
    //NonReferenced Messages (Once done, you can't access them)
    public void ShowPopUpMessage(string header, string message, string button, UnityAction onPress)
    {
        UIPopUp popUpMessage = Instantiate(popUpMessageAsset, gameCanvas.transform).GetComponent<UIPopUp>();

        popUpMessage.header.text = header;
        popUpMessage.message.text = message;
        popUpMessage.button.GetComponentInChildren<TextMeshProUGUI>().text = button;
        popUpMessage.button.onClick.AddListener(onPress);
    }
    public void ShowFloatingMessage(string message, float time, Vector3 startSize, float speed)
    {
        StartCoroutine(this.message(message, time, startSize, speed));
    }
    public void ShowRepeatingMessage(string message, Transform parent, float messageTime, float repeats, ConditionChecker condition)
    {
        StartCoroutine(RepeatMessage_Coroutine(message, parent, messageTime, repeats, condition));
    }
    public void ShowDeclare(string message)
    {
        DeclarationElement element = Instantiate(delcareUIAsset, declarationsArea.transform).GetComponent<DeclarationElement>();
        element.message.text = message;
    }


    //NPC inGame Ui
    Dictionary<GameObject, UIElement_NPC> npcUiContainer = new();
    public void CreateNPCUi(GameObject user, Transform parent)
    {
        UIElement_NPC _npcUi = Instantiate(npcUiElementPrefab, parent.position, Quaternion.identity, threeDCanvas.transform).GetComponent<UIElement_NPC>();

        npcUiContainer.Add(user, _npcUi);

        StartCoroutine(TranslateUiElement(_npcUi.gameObject, parent));
    }
    public void UpateNpcUiElement(GameObject user, string text)
    {
        npcUiContainer[user].levelText.text = text;
    }
    public void DestroyNpcUiElement(GameObject user)
    {
        if (npcUiContainer.ContainsKey(user))
        {
            UIElement_NPC _temp = npcUiContainer[user];
            npcUiContainer.Remove(user);
            Destroy(_temp.gameObject);
        }
    }


    Dictionary<GameObject, UiElement_Slider> sliders  = new();
    public void CreateSlider(GameObject user, Transform parent, float maxValue)
    {
        UiElement_Slider sliderUI = Instantiate(silderElementPrefab, parent.position, Quaternion.identity, threeDCanvas.transform).GetComponentInChildren<UiElement_Slider>();
        sliderUI.SetSliderRange(maxValue);

        sliders.Add(user, sliderUI);
        StartCoroutine(TranslateUiElement(sliderUI.gameObject, parent));
    }
    public void UpdateSlider(GameObject user, float value)
    {
        sliders[user].UpdateSliderValue(value);
    }
    public void DestroySlider(GameObject user)
    {
        if (sliders.ContainsKey(user))
        {
            UiElement_Slider _temp = sliders[user];
            sliders.Remove(user);
            Destroy(_temp.gameObject);
        }
    }


    //Inventory Ui
    public void DisplayInventory(bool state, InventorySystem inventorySystem)
    {
        if(state)
        {
            var items = itemsParent.GetComponentsInChildren<InventoryItemUI>();

            foreach (var item in items)
                Destroy(item.gameObject);

            foreach(var item in inventorySystem.GetInventoryData())
            {
                InventoryItemUI itemUI = Instantiate(inventoryElementUIAsset, itemsParent.transform).GetComponent<InventoryItemUI>();
                itemUI.Intialize(inventorySystem, gameCanvas, item.tag,item.amount,itemUI.OnPress);
            }
        }
    }


    //Control UI flow
    public void OpenMenuPanel(string menuPanelName_PlusManagerNum)
    {
        PanelsManager.OpenMenuPanel(menuPanelName_PlusManagerNum, PanelsManagers, true);
    }
    public void SetPanelDefault(string menuPanelName_PlusManagerNum)
    {
        PanelsManager.SetDefault(menuPanelName_PlusManagerNum, PanelsManagers);
    }
    public void OpenMenuPanelNonExclusive(string menuPanelName_PlusManagerNum)
    {
        PanelsManager.OpenMenuPanel(menuPanelName_PlusManagerNum, PanelsManagers, false);
    }
    public void CloseMenuPanelNonExclusive(string menuPanelName_PlusManagerNum)
    {
        PanelsManager.CloseMenuPanel(menuPanelName_PlusManagerNum, PanelsManagers);
    }
    public void CloseAllPanels()
    {
        foreach(PanelsManager panel in PanelsManagers)
        {
            panel.CloseAll();
        }
    }
    public void ToggleMenuPanel(string menuInfo)
    {
        PanelsManager.TogglePanel(menuInfo, PanelsManagers, false);
    }
    public IEnumerator SavingUI()
    {
        savingText.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        savingText.SetActive(false);
    }
    


    //Interal Algorithms
    IEnumerator message(string message, float time, Vector3 startScale, float speed)
    {
        var messageObj = Instantiate(highlightMessage, messagesArea.transform);
        
        messageObj.transform.localScale = startScale;

        var Text = messageObj.GetComponent<TextMeshProUGUI>();

        Text.text =  message;

        float _animationCurrentTime = time;

        while(_animationCurrentTime >= 0)
        {

            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, _animationCurrentTime / time);

            _animationCurrentTime -= Time.fixedDeltaTime;

            messageObj.transform.localScale += speed * (new Vector3(Time.fixedDeltaTime, Time.fixedDeltaTime, Time.fixedDeltaTime));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    IEnumerator TranslateUiElement(GameObject _object, Transform parent)
    {
        while((parent != null ) && (_object != null))
        {
            _object.transform.position = parent.transform.position + Vector3.up;

            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator RepeatMessage_Coroutine(string message, Transform parent, float messageTime, float repeats, ConditionChecker condition)
    {
        float _time = 0f;

        while (condition.isTrue && _time < messageTime && parent != null)
        {
            _time += messageTime / (repeats);
            SpawnMessage(message, (parent.position + (1.5f*Vector3.up)));
            yield return new WaitForSecondsRealtime(messageTime / repeats);
        }
    }
    void SpawnMessage(string text, Vector3 position)
    {
        var _gameObject = Instantiate(ThreeDHighlightPrefab, position, Quaternion.identity, threeDCanvas.transform);
        _gameObject.GetComponentInChildren<Text>().text = text;
    }
    void ChangeAlpha(Image myImage, bool state)
    {
        myImage.color =  new Color(myImage.color.r, myImage.color.g, myImage.color.b, state ? buttonOnAlpha : buttonOffAlpha);
    }
}


 