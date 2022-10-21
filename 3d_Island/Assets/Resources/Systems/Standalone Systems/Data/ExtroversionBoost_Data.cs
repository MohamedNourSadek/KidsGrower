using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtroversionBoost_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public int currentValue = 0;

    public static List<ExtroversionBoost_Data> GameToDate(ExtroversionBoost[] extroversionBoosts)
    {
        List<ExtroversionBoost_Data> list = new List<ExtroversionBoost_Data>(); 

        foreach (ExtroversionBoost boost in extroversionBoosts)
            list.Add(boost.GetData());

        return list;
    }
}
