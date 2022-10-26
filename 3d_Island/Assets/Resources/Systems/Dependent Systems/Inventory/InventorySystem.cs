using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    public static bool IsStorable(Pickable pickable)
    {
        if (pickable.gameObject.GetComponent<IInventoryItem>() != null)
            return true;
        else
            return false;
    }



    List<IInventoryItem> items = new List<IInventoryItem>();
    IController myController;
     
    public InventorySystem(IController controller)
    {
        myController = controller;
    } 

    public void Add(IInventoryItem item, bool showUI)
    {
        if (items == null)
            items = new List<IInventoryItem>();

        if (!items.Contains(item))
            items.Add(item);

        if (showUI)
            UIGame.instance.ShowRepeatingMessage(
            item.GetGameObject().tag +
            " added to inventory",
            myController.GetBody().transform,
            0.5f, 1, new ConditionChecker(true));

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
    public List<IInventoryItem> GetItems()
    {
        return items;
    }
    public List<InventoryItem_Data> GetItems_Data()
    {
        List<InventoryItem_Data> items_data = new List<InventoryItem_Data>();

        foreach(IInventoryItem item in items)
        {
            bool alreadyExists = false;
            InventoryItem_Data item_data = new InventoryItem_Data();

            foreach(InventoryItem_Data itemData in items_data)
            {
                if(itemData.itemTag == item.GetGameObject().tag)
                {
                    alreadyExists = true;
                    item_data = itemData;
                }
            }

            if(alreadyExists)
            {
                item_data.amount++;
            }
            else
            {
                InventoryItem_Data data = new InventoryItem_Data();

                data.itemTag = item.GetGameObject().tag;
                data.amount = 1;

                items_data.Add(data);
            }

        }

        return items_data;
    }
}




