using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Tearable, IDetectable, ISavable
{
    public GameObject GetGameObject()
    {
        if (this.gameObject == null)
            return null;
        else 
            return this.gameObject;
    }
    
    public void LoadData(SaveStructure savaData)
    {
        Rock_Data rock = (Rock_Data)savaData;
        transform.position = rock.position.GetVector();
        transform.rotation = rock.rotation.GetQuaternion();
        tearingDownCount = rock.tearDownCount;
    }
    public Rock_Data GetData()
    {
        Rock_Data rock_Data = new Rock_Data();
        rock_Data.position = new nVector3(transform.position);
        rock_Data.rotation = new nQuaternion(transform.rotation);
        rock_Data.tearDownCount = tearingDownCount;
        return rock_Data;
    }

    public override void SpawnResource()
    {
        base.SpawnResource();

        GameManager.instance.SpawnStonePack(this.transform.position);
    }
}
