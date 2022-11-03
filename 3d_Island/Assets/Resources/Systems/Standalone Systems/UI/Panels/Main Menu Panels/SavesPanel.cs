using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavesPanel : MenuPanel
{
    [SerializeField] GameObject savePrefab;
    [SerializeField] GameObject savesObject;
    [SerializeField] Button newButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button deleteButton;

    List<GameObject> savesUi = new List<GameObject>();
    GameObject selected;
    int MaxNumOfSaves = 5;

    private void Start()
    {
        UpdateSavesUI();
    }
    private void Update()
    {
        CheckNewSelection();
    }


    //Interface
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
    public SaveInfo GetSelectedSave()
    {
        if (selected && selected.GetComponent<SaveInfo>())
            return selected.GetComponent<SaveInfo>();
        else
            return null;
    }
    public void LoadSave()
    {
        SaveInfo _save = GetSelectedSave();

        DataManager.instance.SetCurrentSession(_save.saveName.text);

        OpenGame();
    }
    public void DeleteSave()
    {
        SaveInfo _save = GetSelectedSave();

        DataManager.instance.Remove(_save.saveName.text);

        UpdateSavesUI();
    }


    //Internal 
    void OpenGame()
    {
        SceneControl.instance.LoadScene(1);
    }
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


        if (selected != null && selected.GetComponent<SaveInfo>())
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
}
