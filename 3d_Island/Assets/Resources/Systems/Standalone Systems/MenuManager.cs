using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void NewSave()
    {
        var _time = System.DateTime.Now;

        SessionData sessionData = new SessionData(UIMenu.instance.GetSaveNameAndRefresh(), DataManager.instance.GetCurrentMode(), _time.ToString(), UIMenu.instance.GetDifficulty());

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
