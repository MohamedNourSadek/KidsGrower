using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<Hat_Data> GameToDate(Hat[] hats)
    {
        List<Hat_Data> list = new List<Hat_Data>();

        foreach (Hat hat in hats)
            if(hat.isWorn == false)
                list.Add(hat.GetData());

        return list;
    }
}
