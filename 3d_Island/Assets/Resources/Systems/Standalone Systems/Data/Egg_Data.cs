using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg_Data 
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();
    public float rottenness = 0f;

    public void SpawnWithData(GameObject eggAsset)
    {
        ServicesProvider.Instantiate(eggAsset).GetComponent<Egg>().LoadData(this);
    }
    public static List<Egg_Data> GameToDate(Egg[] eggs)
    {
        List<Egg_Data> list = new List<Egg_Data>(); 

        foreach (Egg egg in eggs)
            list.Add(egg.GetData());

        return list;
    }
}
