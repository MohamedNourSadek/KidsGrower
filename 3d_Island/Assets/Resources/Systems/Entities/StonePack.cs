using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePack : Pickable, ISavable, IStorableObject, IDetectable
{
    public void LoadData(SaveStructure saveData)
    {
        StonePack_Data stonePackData = (StonePack_Data)saveData;

        transform.position = stonePackData.position.GetVector();
    }
    public StonePack_Data GetData()
    {
        StonePack_Data stonePackData = new StonePack_Data();

        stonePackData.position = new nVector3(transform.position);

        return stonePackData;
    }

}
