using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothCreator_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<ClothCreator_Data> GameToDate(ClothCreator[] clothCreators)
    {
        List<ClothCreator_Data> list = new List<ClothCreator_Data>();

        foreach (ClothCreator clothCreator in clothCreators)
            list.Add(clothCreator.GetData());

        return list;
    }
}
