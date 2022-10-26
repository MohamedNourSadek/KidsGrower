using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using System.Text;

public class DataManager : MonoBehaviour 
{
    string lastScene = "Main";


    public static DataManager instance;


    //Internal
    static string path;
    WholeData dataCache;
    SessionData currentSession;
    modes currentMode;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
            path = Application.persistentDataPath + "/savedDate.nDx";

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
    void SaveData(WholeData wholeData)
    {
        dataCache = wholeData;

        string data = JsonConvert.SerializeObject(wholeData,Formatting.Indented);

        File.WriteAllText(path, Encypt(data));
    }
    void GetData()
    {
        var save = File.ReadAllText(path);
        
        WholeData data = JsonConvert.DeserializeObject<WholeData>(Decrpt(save));

        if (data != null)
            dataCache = data;
        else
            dataCache = new WholeData();
    }
    
    string Encypt(string raw)
    {
        string encrypted = raw;
        return encrypted;
    }
    string Decrpt(string encrypted)
    {
        string raw = encrypted;
        return raw;
    }


    //Interface to access all sessions Data
    public WholeData GetSavedData()
    {
        if(dataCache == null)
        {
            GetData();
        }

        return dataCache;
    }
    public void Add(SessionData sessionData)
    {
        WholeData data = GetSavedData();

        data.sessions.Add(sessionData);

        SaveData(data);
    }
    public void Modify(SessionData newData)
    {
        WholeData data = GetSavedData();
        SessionData oldData = data.sessions.Find(x => x.sessionName == newData.sessionName);
        int i = data.sessions.IndexOf(oldData);
        data.sessions[i] = newData;

        SaveData(data);
    }
    public void Remove(string sessionName)
    {
        WholeData data = GetSavedData();
        SessionData oldData = data.sessions.Find(x => x.sessionName == sessionName);
        data.sessions.Remove(oldData);

        SaveData(data);
    }
    public bool Contains(string sessionName)
    {
        WholeData data = GetSavedData();

        if (data.sessions.Count > 0)
            if (data.sessions.Find(x => x.sessionName == sessionName) != null)
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
        WholeData data = GetSavedData();

        SessionData session = data.sessions.Find(x => x.sessionName == sessionName);

        return session;
    }
    public SessionData GetCurrentSession()
    {
        return currentSession;
    }
    public string GetLastScene()
    {
        return lastScene;
    }
}
