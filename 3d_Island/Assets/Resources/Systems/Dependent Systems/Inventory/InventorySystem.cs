using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    [SerializeField] public List<InventoryItem_Data> items = new List<InventoryItem_Data>();
    IController myController;
     
    public void Initialize(IController controller)
    {
        myController = controller;
    }
    public static bool IsStorable(Pickable pickable)
    {
        if (pickable.gameObject.GetComponent<IStorableObject>() != null)
            return true;
        else
            return false;
    }

    //Interface
    public List<InventoryItem_Data> GetItems_Data()
    {
        return items;
    }
    public void Add(GameObject item, bool showUI)
    {
        string name = item.GetComponent<Pickable>().GetType().ToString();

        if (showUI)
            UIGame.instance.ShowRepeatingMessage(
            name +
            " added to inventory",
            myController.GetBody().transform,
            0.5f, 1, new ConditionChecker(true));

        ModifyAmount(name, 1);
        ServicesProvider.instance.DestroyObject(item); 
    }
    public void Remove(string itemName)
    {
        UIGame.instance.ShowRepeatingMessage(
            itemName + " removed from inventory",
            myController.GetBody().transform,
            0.5f, 1, new ConditionChecker(true));

        ModifyAmount(itemName, -1);
        DestroyEmpty();
        DropInventoryItem(itemName);
    }
    public void DropInventoryItem(string item)
    {
        if (item == "Harvest")
            GameManager.instance.SpawnHarvest();
        else if (item == "Fruit")
            GameManager.instance.SpawnFruit();
        else if (item == "StonePack")
            GameManager.instance.SpawnStonePack();
        else if (item == "WoodPack")
            GameManager.instance.SpawnWoodPack();
        else if (item == "Seed")
            GameManager.instance.SpawnSeed();
        else if(item.Contains("Boost"))
            GameManager.instance.SpawnBoost(item);
    }
    public void LoadInventory(List<InventoryItem_Data> data)
    {
        items = data;
    }
    public List<InventoryItem_Data> GetInventoryData()
    {
        return items;
    }

    //Internal Algorithms
    bool Exists(string tag)
    {
        if(items != null)
        {
            foreach (InventoryItem_Data item in items)
                if (item.tag == tag)
                    return true;
        }

        return false;
    }
    void ModifyAmount(string tag, int increment)
    {
        if (items == null)
        {
            items = new List<InventoryItem_Data>();
        }

        if (Exists(tag))
        {
            foreach (InventoryItem_Data item in items)
            {
                if (item.tag == tag)
                {
                    item.amount += increment;
                }
            }
        }
        else
        {
            if(increment>0)
                items.Add(new InventoryItem_Data() { amount = 1, tag = tag});
        }
    }
    void DestroyEmpty()
    {
        List<InventoryItem_Data> emptyItems = new List<InventoryItem_Data>();

        foreach (InventoryItem_Data item in items)
            if (item.amount == 0)
                emptyItems.Add(item);

        foreach (InventoryItem_Data emptyItem in emptyItems)
            items.Remove(emptyItem);
    }

}




