using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Pickable, ISavable
{
    public void LoadData(SaveStructure saveStructure)
    {
        Ball_Data ballData = (Ball_Data)saveStructure;
        
        transform.position = ballData.position.GetVector();
    }
    public Ball_Data GetData()
    {
        Ball_Data ball_data = new Ball_Data();

        ball_data.position = new nVector3(transform.position);

        return ball_data;
    }
}
