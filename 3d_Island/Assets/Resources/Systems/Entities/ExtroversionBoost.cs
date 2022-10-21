using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtroversionBoost : Eatable, ISavable
{
    public void LoadData(SaveStructure saveData)
    {
        ExtroversionBoost_Data boost_data = (ExtroversionBoost_Data)saveData;

        transform.position = boost_data.position.GetVector();
        currentValue = boost_data.currentValue;
    }
    public ExtroversionBoost_Data GetData()
    {
        ExtroversionBoost_Data boost_data = new ExtroversionBoost_Data();

        boost_data.position = new nVector3(transform.position);
        boost_data.currentValue = currentValue;

        return boost_data;
    }
    public override void ApplyEffect(NPC holder)
    {
        base.ApplyEffect(holder);
        holder.character.extroversionFactor += (0.001f * GetMore());
    }
}
