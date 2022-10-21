using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBoost_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public int currentValue = 0;

    public static List<PowerBoost_Data> GameToDate(PowerBoost[] fertilityBoosts)
    {
        List<PowerBoost_Data> list = new List<PowerBoost_Data>(); 

        foreach (PowerBoost boost in fertilityBoosts)
            list.Add(boost.GetData());

        return list;
    }
}
