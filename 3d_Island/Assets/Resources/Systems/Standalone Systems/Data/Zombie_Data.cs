using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();
    public int health = 30;

    public static List<Zombie_Data> GameToDate(Zombie[] zombies)
    {
        List<Zombie_Data> list = new List<Zombie_Data>();

        foreach (Zombie zombie in zombies)
            list.Add(zombie.GetData());

        return list;
    }
}
