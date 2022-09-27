using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();
    public List<InventoryItem_Data> inventoryData = new List<InventoryItem_Data>();

    public static Player_Data GameToData(PlayerSystem player)
    {
        return player.GetData();
    }
} 
 