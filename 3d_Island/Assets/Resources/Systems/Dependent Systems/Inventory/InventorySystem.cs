using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    List<IInventoryItem> items = new();
    IController myController;

    public InventorySystem(IController controller)
    {
        myController = controller;
    }
    public void Add(IInventoryItem item)
    {
        if (!items.Contains(item))
            items.Add(item);

        UIGame.instance.ShowRepeatingMessage(
            item.GetGameObject().tag + " added to inventory",
            myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));

        item.GetGameObject().transform.position = new Vector3(5000f, 5000f, 5000f);
    }
    public void Remove(IInventoryItem item)
    {
        if (items.Contains(item))
            items.Remove(item);

        UIGame.instance.ShowRepeatingMessage(
            item.GetGameObject().tag + " removed from inventory",
                myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));

        item.GetGameObject().transform.position = myController.GetBody().transform.position;
    }
    public static bool IsStorable(Pickable pickable)
    {
        if(pickable.gameObject.GetComponent<IInventoryItem>() != null)
            return true;
        else
            return false;
    }
    public List<IInventoryItem> GetItems()
    {
        return items;
    }
}




