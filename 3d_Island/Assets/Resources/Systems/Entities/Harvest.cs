using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : Pickable, IInventoryItem
{
    public void LoadData(Harvest_Data harvest_data)
    {
        transform.position = harvest_data.position.GetVector();
    }
    public Harvest_Data GetData()
    {
        Harvest_Data harvestData = new Harvest_Data();

        harvestData.position = new nVector3(transform.position);

        return harvestData;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
