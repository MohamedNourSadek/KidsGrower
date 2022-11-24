using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Pickable, ISavable, IStorableObject, IDetectable
{
    public int damage = 10;
    public int attackTime = 1;

    public void LoadData(SaveStructure saveData)
    {
        Sword_Data sword = (Sword_Data)saveData;
        transform.position = sword.position.GetVector();
    }
    public Sword_Data GetData()
    {
        Sword_Data sword = new Sword_Data();
        sword.position = new nVector3(transform.position);
        return sword;
    }
}

