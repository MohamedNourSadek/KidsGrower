using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Pickable
{
    public void LoadData(Ball_Data ball_data)
    {
        transform.position = ball_data.position.GetVector();
    }
    public Ball_Data GetData()
    {
        Ball_Data ball_data = new Ball_Data();

        ball_data.position = new nVector3(transform.position);

        return ball_data;
    }
}
