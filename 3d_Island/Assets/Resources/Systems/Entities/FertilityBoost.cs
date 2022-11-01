using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilityBoost : Eatable, ISavable, IStorableObject
{
    public void LoadData(SaveStructure saveData)
    {
        FertilityBoost_Data boost_data = (FertilityBoost_Data)saveData;

        transform.position = boost_data.position.GetVector();
        currentValue = boost_data.currentValue;
    }
    public FertilityBoost_Data GetData()
    {
        FertilityBoost_Data boost_data = new FertilityBoost_Data();

        boost_data.position = new nVector3(transform.position);
        boost_data.currentValue = currentValue;

        return boost_data;
    }

    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);
        holder.character.fertilityFactor += (0.001f * GetMore());
    }
}
