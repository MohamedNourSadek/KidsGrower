using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit_Data 
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public void SpawnWithData(GameObject fruitAsset)
    {
        ServicesProvider.Instantiate(fruitAsset).GetComponent<Fruit>().LoadData(this);
    }
    public static List<Fruit_Data> GameToDate(Fruit[] fruits)
    {
        List<Fruit_Data> list = new List<Fruit_Data>(); 

        foreach (Fruit fruit in fruits)
            if(fruit.OnGround())
                list.Add(fruit.GetData());

        return list;
    }
}
