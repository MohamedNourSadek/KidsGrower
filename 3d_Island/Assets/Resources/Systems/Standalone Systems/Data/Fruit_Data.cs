using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<Fruit_Data> GameToDate(Fruit[] fruits)
    {
        List<Fruit_Data> list = new List<Fruit_Data>(); 

        foreach (Fruit fruit in fruits)
            if(fruit.groundDetector.IsOnLayer(GroundLayers.Ground))
                list.Add(fruit.GetData());

        return list;
    }
}
