using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SessionData
{
    public string sessionName;
    public string modeName;
    public string since;
    public GameData data = new GameData();

    public SessionData(string sessionName, string modeName, string since)
    {
        this.sessionName = sessionName;
        this.modeName = modeName;
        this.since = since;
        this.data = new GameData() { player = new Player_Data(), npcs = new List<NPC_Data>()};
    }
}
