using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoost : Eatable, ISavable, IStorableObject
{
    public void LoadData(SaveStructure saveData)
    {
        HealthBoost_Data boost_data = (HealthBoost_Data)saveData;

        transform.position = boost_data.position.GetVector();
        currentValue = boost_data.currentValue;
    }
    public HealthBoost_Data GetData()
    {
        HealthBoost_Data boost_data = new HealthBoost_Data();

        boost_data.position = new nVector3(transform.position);
        boost_data.currentValue = currentValue;

        return boost_data;
    }
    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);
        holder.character.healthFactor += (0.001f * GetMore());
    } 
}
