using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilityBoost_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public int currentValue = 0;

    public static List<FertilityBoost_Data> GameToDate(FertilityBoost[] fertilityBoosts)
    {
        List<FertilityBoost_Data> list = new List<FertilityBoost_Data>(); 

        foreach (FertilityBoost boost in fertilityBoosts)
            list.Add(boost.GetData());

        return list;
    }
}
