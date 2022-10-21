using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBoost : Eatable, ISavable
{
    public void LoadData(SaveStructure saveData)
    {
        PowerBoost_Data boost_data = (PowerBoost_Data)saveData;

        transform.position = boost_data.position.GetVector();
        currentValue = boost_data.currentValue;
    }
    public PowerBoost_Data GetData()
    {
        PowerBoost_Data boost_data = new PowerBoost_Data();

        boost_data.position = new nVector3(transform.position);
        boost_data.currentValue = currentValue;

        return boost_data;
    }

    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);
        holder.character.powerFactor += (0.001f * GetMore());
    }
}
