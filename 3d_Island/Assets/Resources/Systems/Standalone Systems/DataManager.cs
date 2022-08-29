using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

public class DataManager : MonoBehaviour 
{
    string lastScene = "Main";


    public static DataManager instance;


    //Internal
    static string path;
    List<SessionData> dataCache;
    SessionData currentSession;
    modes currentMode;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
            path = Application.persistentDataPath + "/savedDate.json";

            if (File.Exists(path) == false)
                File.Create(path);
            else
                GetSavedData();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void SaveData(List<SessionData> sessionsData)
    {
        dataCache = sessionsData;

        string data = JsonConvert.SerializeObject(sessionsData);

        File.WriteAllText(path, data);
    }


    //Interface to access all sessions Data
    public List<SessionData> GetSavedData()
    {
        if(dataCache == null)
        {
            string save = File.ReadAllText(path);
            
            List<SessionData> data = JsonConvert.DeserializeObject<List<SessionData>>(save);

            if(data != null)
                dataCache = data;
            else 
                dataCache = new List<SessionData>(); 
        }

        return dataCache;
    }
    public void Add(SessionData sessionData)
    {
        List<SessionData> list = GetSavedData();

        list.Add(sessionData);

        SaveData(list);
    }
    public void Remove(string sessionName)
    {
        List<SessionData> list = GetSavedData();
        SessionData oldData = list.Find(x => x.sessionName == sessionName);
        list.Remove(oldData);

        SaveData(list);
    }
    public bool Contains(string sessionName)
    {
        List<SessionData> list = GetSavedData();

        if (list.Count > 0)
            if (list.Find(x => x.sessionName == sessionName) != null)
                return true;
            else
                return false;
        else
            return false;
    }



    //Interface to access currentSessionData
    public void SetCurrentSession(string sessionName)
    {
        currentSession = GetSessionData(sessionName);
    }
    public void SetCurrentMode(string modeName)
    {
        currentMode = ModesEnums.GetEnumFromString(modeName);
    }
    public void SetLastScenen(string _lastScenen)
    {
        lastScene = _lastScenen;
    }
    public modes GetCurrentMode()
    {
        return currentMode;
    }
    public SessionData GetSessionData(string sessionName)
    {
        List<SessionData> list = GetSavedData();

        SessionData data = list.Find(x => x.sessionName == sessionName);

        return data;
    }
    public SessionData GetCurrentSession()
    {
        return currentSession;
    }
    public string GetLastScene()
    {
        return lastScene;
    }
    public void Modify(SessionData newData)
    {
        List<SessionData> list = GetSavedData();
        SessionData oldData = list.Find(x => x.sessionName == newData.sessionName);
        int i = list.IndexOf(oldData);
        list[i] = newData;

        SaveData(list);
    }
}
