using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressivenessBoost : Eatable, ISavable
{
    public void LoadData(SaveStructure saveData)
    {
        AggressivenessBoost_Data boost_data = (AggressivenessBoost_Data)saveData;

        transform.position = boost_data.position.GetVector();
        currentValue = boost_data.currentValue;
    }
    public AggressivenessBoost_Data GetData()
    {
        AggressivenessBoost_Data boost_data = new AggressivenessBoost_Data();

        boost_data.position = new nVector3(transform.position);
        boost_data.currentValue = currentValue;

        return boost_data;
    }
    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);
        holder.character.aggressivenessFactor += (0.001f * GetMore());
    }

}
