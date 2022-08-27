using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Data
{
    public nVector3 position = new nVector3();

    public void SpawnWithData(GameObject ballAsset)
    {
        ServicesProvider.Instantiate(ballAsset).GetComponent<Ball>().LoadData(this);
    }
    public static List<Ball_Data> GameToDate(Ball[] balls)
    {
        List<Ball_Data> list = new List<Ball_Data>();

        foreach (Ball ball in balls)
            list.Add(ball.GetData());

        return list;
    }
}
