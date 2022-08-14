using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvest : Pickable, IInventoryItem
{
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
