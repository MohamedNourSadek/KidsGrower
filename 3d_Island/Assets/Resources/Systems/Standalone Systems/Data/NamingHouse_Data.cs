using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamingHouse_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<NamingHouse_Data> GameToDate(NamingHouse[] nameHouses)
    {
        List<NamingHouse_Data> list = new List<NamingHouse_Data>();

        foreach (NamingHouse nameHouse in nameHouses)
            list.Add(nameHouse.GetData());

        return list;
    }
}
