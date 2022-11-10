using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();
    public int tearDownCount = 0;

    public static List<Rock_Data> GameToDate(Rock[] rocks)
    {
        List<Rock_Data> list = new List<Rock_Data>();

        foreach (Rock rock in rocks)
            list.Add(rock.GetData());

        return list;
    }
}
