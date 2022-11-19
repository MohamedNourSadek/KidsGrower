using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : Pickable, IStorableObject, ISavable
{
    public bool isWorn = false;

    public void LoadData(SaveStructure saveData)
    {
        Hat_Data hat_Data = (Hat_Data)saveData;

        transform.position = hat_Data.position.GetVector();
    }
    public Hat_Data GetData()
    {
        Hat_Data hatData = new Hat_Data();

        hatData.position = new nVector3(transform.position);

        return hatData;
    }
}
