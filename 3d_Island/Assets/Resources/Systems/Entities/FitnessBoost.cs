using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessBoost : Eatable, ISavable, IStorableObject
{
    public void LoadData(SaveStructure saveData)
    {
        FitnessBoost_Data boost_data = (FitnessBoost_Data)saveData;

        transform.position = boost_data.position.GetVector();
        currentValue = boost_data.currentValue;
    }
    public FitnessBoost_Data GetData()
    {
        FitnessBoost_Data boost_data = new FitnessBoost_Data();

        boost_data.position = new nVector3(transform.position);
        boost_data.currentValue = currentValue;

        return boost_data;
    }
    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);
        holder.character.fitness += (0.001f * GetMore());
    } 
}
