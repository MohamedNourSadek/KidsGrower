using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Pickable, ISavable, IStorableObject, IDetectable
{
    public void LoadData(SaveStructure saveData)
    {
        Axe_Data axe = (Axe_Data)saveData;
        transform.position = axe.position.GetVector();
    }
    public Axe_Data GetData()
    {
        Axe_Data axe = new Axe_Data();
        axe.position = new nVector3(transform.position);
        return axe;
    }
}

