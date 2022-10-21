using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoost_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public int currentValue = 0;

    public static List<HealthBoost_Data> GameToDate(HealthBoost[] fertilityBoosts)
    {
        List<HealthBoost_Data> list = new List<HealthBoost_Data>(); 

        foreach (HealthBoost boost in fertilityBoosts)
            list.Add(boost.GetData());

        return list;
    }
}
