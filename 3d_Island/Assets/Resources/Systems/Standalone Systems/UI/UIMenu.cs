using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class UIMenu : MonoBehaviour, IPanelsManagerUser
{
    public static UIMenu instance;

    [SerializeField] PanelsManager panelsManager;
    [SerializeField] PostProcessingFunctions postProcessingFunctions;

    [Header("References")]
    [SerializeField] GameObject savePrefab;
    [SerializeField] GameObject savesObject;
    [SerializeField] Button newButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button deleteButton;
    [SerializeField] TMP_InputField saveNameInput;
    [SerializeField] Button createSave;
    [SerializeField] TextMeshProUGUI inputHint;

    List<GameObject> savesUi = new List<GameObject>();
    GameObject selected;
    int MaxNumOfSaves = 5;


    //private Functions
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        panelsManager.Initialize();
        postProcessingFunctions.Initialize();
        postProcessingFunctions.SetBlur(true);
        saveNameInput.onValueChanged.AddListener(SaveNameVaildator);

        UpdateSavesUI();
    }
    void OnDrawGizmos()
    {
        panelsManager.OnDrawGizmos();
    }
    void Update()
    {
        CheckNewSelection();
    }


    //UI commands
    public void OpenMenuPanel(string _menuName)
    {
        panelsManager.OpenMenuPanel(_menuName, true);
    }
    public void ToggleMenuPanel(string _menuInfo)
    {
        panelsManager.ToggleMenuPanel(_menuInfo, false);
    }
    public void OpenMenuPanelNonExclusive(string _menuInfo)
    {
        panelsManager.OpenMenuPanel(_menuInfo, false);
    }
    public void CloseMenuPanelNonExclusive(string _menuInfo)
    {
        panelsManager.CloseMenuPanel(_menuInfo);
    }
    public void SetLastScene(string sceneName)
    {
        DataManager.instance.SetLastScenen(sceneName);
    }
    public void SetCurrentMode(string modeName)
    {
        DataManager.instance.SetCurrentMode(modeName);
    }
    public void UpdateSavesUI()
    {
        ClearOldUI();

        List<SessionData> saves = DataManager.instance.GetSavedData().sessions;

        //Recreate them and add
        foreach (var _save in saves)
        {
            if (_save.modeData.modeName == DataManager.instance.GetCurrentMode())
            {
                var _saveInfo = Instantiate(savePrefab, savesObject.transform).GetComponent<SaveInfo>();
                _saveInfo.saveName.text = _save.sessionName;
                _saveInfo.date.text = _save.since;
                savesUi.Add(_saveInfo.gameObject);
            }
        }

        UpdateSavesButton();
    }
    public string GetSaveNameAndRefresh()
    {
        string save = saveNameInput.text;
        saveNameInput.text = "";
        return save;
    }
    public SaveInfo GetSelectedSave()
    {
        if (selected && selected.GetComponent<SaveInfo>())
            return selected.GetComponent<SaveInfo>();
        else
            return null;
    }


    //internal
    void ClearOldUI()
    {
        foreach (GameObject saveUi in savesUi)
        {
            Destroy(saveUi);
        }
        savesUi.Clear();
    }
    void UpdateSavesButton()
    {
        if (savesUi.Count >= MaxNumOfSaves)
        {
            newButton.interactable = false;
        }
        else
        {
            newButton.interactable = true;
        }


        if(selected != null && selected.GetComponent<SaveInfo>())
        {
            loadButton.interactable = true;
            deleteButton.interactable = true;
        }
        else
        {
           loadButton.interactable = false;
           deleteButton.interactable = false;
        }

    }
    void CheckNewSelection()
    {
        var _new = EventSystem.current.currentSelectedGameObject;

        if (_new != selected)
        {
            if (_new != null)
            {
                if (_new.GetComponent<SaveInfo>())
                    selected = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                selected = null;
            }

            UpdateSavesButton();
        }

    }
    void SaveNameVaildator(string _saveName)
    {
        string _saveNameVaildator = "";

        if(_saveName.Length >= 15)
        {
            _saveNameVaildator = "Save name is too long";
        }
        else if(DataManager.instance.Contains(_saveName))
        {
            _saveNameVaildator = "Save name already exists";
        }
        else if(_saveName.Replace(" ", "").Length == 0)
        {
            _saveNameVaildator = "Enter a name";
        }


        if(_saveNameVaildator == "")
        {
            createSave.interactable = true;
            inputHint.text = "";
        }
        else
        {
            createSave.interactable = false;
            inputHint.text = _saveNameVaildator;
        }
    }
}





