using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<Axe_Data> GameToDate(Axe[] axes)
    {
        List<Axe_Data> list = new List<Axe_Data>();

        foreach (Axe axe in axes)
            list.Add(axe.GetData());

        return list;
    }
}
