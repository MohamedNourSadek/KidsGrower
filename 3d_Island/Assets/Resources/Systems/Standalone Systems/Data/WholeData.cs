using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WholeData
{
    [SerializeField] public SettingsData settings = new SettingsData();
    [SerializeField] public List<SessionData> sessions = new List<SessionData>();


    public WholeData()
    {
        this.sessions = new List<SessionData>();
    }
    public WholeData(List<SessionData> data)
    {
        this.sessions = data;
    }
}
