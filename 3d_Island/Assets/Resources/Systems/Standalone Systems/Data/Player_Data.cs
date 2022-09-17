using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static Player_Data GameToData(PlayerSystem player)
    {
        return player.GetData();
    }
} 
 