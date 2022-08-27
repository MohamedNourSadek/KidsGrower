using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Data
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public void SpawnWithData(GameObject playerObj)
    {
        playerObj.GetComponent<PlayerSystem>().LoadData(this);
    }
    public static Player_Data GameToData(PlayerSystem player)
    {
        return player.GetData();
    }
}
