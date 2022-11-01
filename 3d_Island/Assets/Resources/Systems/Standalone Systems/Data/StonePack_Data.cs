using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePack_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<StonePack_Data> GameToDate(StonePack[] stonePacks)
    {
        List<StonePack_Data> list = new List<StonePack_Data>();

        foreach (StonePack stonePack in stonePacks)
            list.Add(stonePack.GetData());

        return list;
    }
}
