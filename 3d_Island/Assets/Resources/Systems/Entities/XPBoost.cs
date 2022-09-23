using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPBoost : Pickable, IInventoryItem
{
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
