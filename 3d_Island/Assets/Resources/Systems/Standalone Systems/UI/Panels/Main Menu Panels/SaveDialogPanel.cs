using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveDialogPanel : MenuPanel
{
    [SerializeField] TMP_InputField saveNameInput;
    [SerializeField] Button createSave;
    [SerializeField] TextMeshProUGUI inputHint;
    [SerializeField] SavesPanel savesPanel;

    private void Start()
    {
        saveNameInput.onValueChanged.AddListener(SaveNameVaildator);
    }

    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Cancel").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Game Data")));
        
        GetButton("Create").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Game Data")));
        GetButton("Create").onClick.AddListener(new UnityAction(() => NewSave()));
        GetButton("Create").onClick.AddListener(new UnityAction(() => savesPanel.UpdateSavesUI()));
    }
    public string GetSaveNameAndRefresh()
    {
        string save = saveNameInput.text;
        saveNameInput.text = "";
        return save;
    }
    public void NewSave()
    {
        var _time = System.DateTime.Now;
        SessionData sessionData = new SessionData(GetSaveNameAndRefresh(), DataManager.instance.GetCurrentMode(), _time.ToString());
        DataManager.instance.Add(sessionData);
    }


    //internal
    void SaveNameVaildator(string _saveName)
    {
        string _saveNameVaildator = "";

        if (_saveName.Length >= 15)
        {
            _saveNameVaildator = "Save name is too long";
        }
        else if (DataManager.instance.Contains(_saveName))
        {
            _saveNameVaildator = "Save name already exists";
        }
        else if (_saveName.Replace(" ", "").Length == 0)
        {
            _saveNameVaildator = "Enter a name";
        }


        if (_saveNameVaildator == "")
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
