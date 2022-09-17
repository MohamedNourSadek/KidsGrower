using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SaveStructure
{
    public virtual void SpawnWithData(GameObject objAsset, bool spawn)
    {
        GameObject obj;

        if (spawn)
            obj = ServicesProvider.Instantiate(objAsset);
        else
            obj = objAsset;

        obj.GetComponent<ISavable>().LoadData(this);
    }
}
