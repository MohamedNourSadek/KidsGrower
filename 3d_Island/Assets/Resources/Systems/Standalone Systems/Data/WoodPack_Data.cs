using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPack_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<WoodPack_Data> GameToDate(WoodPack[] woodPacks)
    {
        List<WoodPack_Data> list = new List<WoodPack_Data>();

        foreach (WoodPack woodPack in woodPacks)
            list.Add(woodPack.GetData());

        return list;
    }
}
