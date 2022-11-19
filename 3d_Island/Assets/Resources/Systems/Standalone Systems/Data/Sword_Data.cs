using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<Sword_Data> GameToDate(Sword[] swords)
    {
        List<Sword_Data> list = new List<Sword_Data>();

        foreach (Sword sword in swords)
            list.Add(sword.GetData());

        return list;
    }
}
