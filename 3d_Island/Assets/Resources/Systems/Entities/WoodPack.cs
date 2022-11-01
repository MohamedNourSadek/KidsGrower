using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPack : Pickable, ISavable, IStorableObject, IDetectable
{
    public void LoadData(SaveStructure saveData)
    {
        WoodPack_Data woodPackData = (WoodPack_Data)saveData;

        transform.position = woodPackData.position.GetVector();
    }
    public WoodPack_Data GetData()
    {
        WoodPack_Data woodPackData = new WoodPack_Data();

        woodPackData.position = new nVector3(transform.position);

        return woodPackData;
    }

}
