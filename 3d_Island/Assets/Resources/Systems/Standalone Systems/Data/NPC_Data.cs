using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Data : SaveStructure
{
    public string name = "Nameless";
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();
    public float bornSince = 0;
    public float xp = 0;
    public float lastLaidSince = 50000;
    public List<AIParameter> aIParameters = new List<AIParameter>();


    public NPC_Data(AiSet set)
    {
        foreach (var aiName in Enum.GetValues(typeof(AIParametersNames)))
            aIParameters.Add(new AIParameter() { saveName = aiName.ToString(), value = AIParameter.GetParameterDefaultValue(aiName.ToString(), set) });
    }
    public static List<NPC_Data> GameToDate(NPC[] npcs)
    {
        List<NPC_Data> list = new List<NPC_Data>(); 

        foreach (NPC npc in npcs)
            list.Add(npc.GetData());

        return list;
    }
}
