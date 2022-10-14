using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    private void Awake()
    {
        StartCoroutine(LoadData());
    }
    public IEnumerator LoadData()
    {
        yield return new WaitForSecondsRealtime(1f);
        SettingsData settingsData = DataManager.instance.GetSavedData().settings;

        SetAmbientVolume(settingsData.ambinetVolume);
        SetUIVolume(settingsData.uiVolume);
        SetSFXVolume(settingsData.sfxVolume);

    }


    public void SetSFXVolume(float volume)
    {
        SoundManager.instance.SetSFX(volume);
    }
    public void SetAmbientVolume(float volume)
    {
        SoundManager.instance.SetAmbient(volume);
    }
    public void SetUIVolume(float volume)
    {
        SoundManager.instance.SetUiVolume(volume);
    }


    public void NewSave()
    {
        var _time = System.DateTime.Now;

        SessionData sessionData = new SessionData(UIMenu.instance.GetSaveNameAndRefresh(), DataManager.instance.GetCurrentMode(), _time.ToString());

        DataManager.instance.Add(sessionData);

        UIMenu.instance.UpdateSavesUI();
    }
    public void LoadSave()
    {
        SaveInfo _save = UIMenu.instance.GetSelectedSave();

        DataManager.instance.SetCurrentSession(_save.saveName.text);

        OpenGame();
    }
    public void DeleteSave()
    {
        SaveInfo _save = UIMenu.instance.GetSelectedSave();

        DataManager.instance.Remove(_save.saveName.text);

        UIMenu.instance.UpdateSavesUI();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OpenGame()
    {
        SceneControl.instance.LoadScene(1);
    }


}
