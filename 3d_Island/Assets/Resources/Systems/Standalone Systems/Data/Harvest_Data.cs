using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<Harvest_Data> GameToDate(Harvest[] harvests)
    {
        List<Harvest_Data> list = new List<Harvest_Data>();

        foreach (Harvest harvest in harvests)
            list.Add(harvest.GetData());

        return list;
    }
}
