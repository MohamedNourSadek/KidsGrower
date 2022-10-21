using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressivenessBoost_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public int currentValue = 0;

    public static List<AggressivenessBoost_Data> GameToDate(AggressivenessBoost[] aggressivenessBoost)
    {
        List<AggressivenessBoost_Data> list = new List<AggressivenessBoost_Data>(); 

        foreach (AggressivenessBoost boost in aggressivenessBoost)
            list.Add(boost.GetData());

        return list;
    }
}
