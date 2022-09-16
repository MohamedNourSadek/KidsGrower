using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SessionData
{
    public string sessionName;
    public string since;
    public Mode_Data modeData = new Mode_Data();
    public GameData data = new GameData();
    public List<AIParameter> aIParameters = new List<AIParameter>();

    public SessionData(string sessionName, modes modeName, string since, AiSet set)
    {
        this.sessionName = sessionName;
        this.since = since;
        this.data = new GameData() { player = new Player_Data(), npcs = new List<NPC_Data>()};
        this.modeData = new Mode_Data(modeName, 0f);

        foreach (var aiName in Enum.GetValues(typeof(AIParametersNames)))
            aIParameters.Add(new AIParameter() { saveName = aiName.ToString(), value = AIParameter.GetParameterDefaultValue(aiName.ToString(), set)});
    }
}
