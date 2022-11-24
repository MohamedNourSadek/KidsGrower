using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessBoost_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public int currentValue = 0;

    public static List<FitnessBoost_Data> GameToDate(FitnessBoost[] fertilityBoosts)
    {
        List<FitnessBoost_Data> list = new List<FitnessBoost_Data>(); 

        foreach (FitnessBoost boost in fertilityBoosts)
            list.Add(boost.GetData());

        return list;
    }
}
