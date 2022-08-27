using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrossScenes : MonoBehaviour
{
    public static CrossScenes instance;

    
    [SerializeField] string LastScreen = "";
    [SerializeField] string CurrentMode = "";
    
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }


    public void SetLastScreen(string screenName)
    {
        LastScreen = screenName;
    }
    public string GetLastScreen()
    {
        return LastScreen;
    }


}
