using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Eatable, ISavable, IStorableObject
{
    public void LoadData(SaveStructure saveData)
    {
        Fruit_Data fruit_data = (Fruit_Data)saveData;

        transform.position = fruit_data.position.GetVector();
    }
    public Fruit_Data GetData()
    {
        Fruit_Data fruit_data = new Fruit_Data();

        fruit_data.position = new nVector3(transform.position);

        return fruit_data;
    }

    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);

        holder.character.levelControl.IncreaseXP(GetMore());
    }
}
