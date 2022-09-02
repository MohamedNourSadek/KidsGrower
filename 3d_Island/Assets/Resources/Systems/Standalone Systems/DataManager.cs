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
    List<SessionData> dataCache;
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
    void SaveData(List<SessionData> sessionsData)
    {
        dataCache = sessionsData;

        string data = JsonConvert.SerializeObject(sessionsData);

        File.WriteAllBytes(path, VeryBasicEncypt(data));
    }
    void GetData()
    {
        var save = File.ReadAllBytes(path);
        
        List<SessionData> data = JsonConvert.DeserializeObject<List<SessionData>>(VeryBasicDecrpt(save));

        if (data != null)
            dataCache = data;
        else
            dataCache = new List<SessionData>();
    }

    
    Dictionary<char, char> encryption = new Dictionary<char, char>()
    {
        {'a','g' }, {'b','n' }, {'c','u'}, {'d','@' }, {'e','*' }, {'f','~'}, 
        {'g','f' }, {'h','m' }, {'i','t'}, {'j','!' }, {'k','&' }, {'l',':'}, 
        {'m','e' }, {'n','l' }, {'o','s'}, {'p','z' }, {'q','^' }, {'r','+'},
        {'s','d' }, {'t','k' }, {'u','r'}, {'v','y' }, {'w','%' }, {'y','_'},
        {'z','c' }, {'{','j' }, {'}','q'}, {'[','x' }, {']','$' }, {',',')'}, {' ','<'},
        {'1','b' }, {'2','i' }, {'3','p'}, {'4','w' }, {'5','#' }, {'6','('},
        {'7','a' }, {'8','h' }, {'9','o'}, {'.','v' }, 
    };
    byte[] VeryBasicEncypt(string raw)
    {
        string encrypted = "";

        foreach(var c in raw)
        {
            bool found = false;

            foreach(var enc in encryption)
            {
                if(c == enc.Key)
                {
                    found = true;
                    encrypted += enc.Value;
                }
            }

            if (found == false)
                encrypted += c;
        }

        byte[] bytes = Encoding.ASCII.GetBytes(encrypted);

        return bytes;
    }
    string VeryBasicDecrpt(byte[] encrypted)
    {
        string encryptedStr = Encoding.ASCII.GetString(encrypted);
        string raw = "";

        foreach(var c in encryptedStr)
        {
            bool found = false;

            foreach (var enc in encryption)
            {
                if (c == enc.Value)
                {
                    found = true;
                    raw += enc.Key;
                }
            }

            if (found == false)
                raw += c;
        }


        return raw;
    }


    //Interface to access all sessions Data
    public List<SessionData> GetSavedData()
    {
        if(dataCache == null)
        {
            GetData();
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
