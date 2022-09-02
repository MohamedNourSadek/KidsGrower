using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed_Data
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public void SpawnWithData(GameObject seedAsset)
    {
        ServicesProvider.Instantiate(seedAsset).GetComponent<Seed>().LoadData(this);
    }
    public static List<Seed_Data> GameToDate(Seed[] seeds)
    {
        List<Seed_Data> list = new List<Seed_Data>();

        foreach (Seed seed in seeds)
            list.Add(seed.GetData());

        return list;
    }
}