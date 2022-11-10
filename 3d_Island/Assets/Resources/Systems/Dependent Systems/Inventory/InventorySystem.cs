using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class InventorySystem
{
    List<InventoryItem_Data> items = new List<InventoryItem_Data>();
    IController myController;
    
    public void Initialize(IController controller)
    {
        myController = controller;
    }


    //Save Game
    public void LoadSavedData(List<InventoryItem_Data> data)
    {
        items = data;
    }
    public List<InventoryItem_Data> GetInventoryData()
    {
        return items;
    }


    //Interface
    public static bool IsStorable(Pickable item)
    {
        if (item.gameObject.GetComponent<IStorableObject>() != null)
            return true;
        else
            return false;
    }
    public void Store(GameObject item, bool showUI)
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
    public GameObject Drop(string itemName)
    {
        UIGame.instance.ShowRepeatingMessage(
            itemName + " removed from inventory",
            myController.GetBody().transform,
            0.5f, 1, new ConditionChecker(true));

        ModifyAmount(itemName, -1);
        DestroyEmpty();
        return SpawnInventoryItem(itemName);
    }
    public bool Exists(string tag)
    {
        if (items != null)
        {
            foreach (InventoryItem_Data item in items)
                if (item.tag == tag)
                    return true;
        }

        return false;
    }
    public int GetAmount(string tag)
    {
        if (items != null)
        {
            foreach (InventoryItem_Data item in items)
                if (item.tag == tag)
                    return item.amount;
        }

        return 0;
    }
    public void ConsumeResources(List<RequirementData> requirements)
    {
        foreach(RequirementData requirement in requirements)
        {
            ModifyAmount(requirement.itemTag, -requirement.itemAmount);
        }
    }


    //Internal Algorithms
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
    GameObject SpawnInventoryItem(string item)
    {
        GameObject myObject = null;

        if (item == "Harvest")
            myObject = GameManager.instance.SpawnHarvest();
        else if (item == "Fruit")
            myObject = GameManager.instance.SpawnFruit();
        else if (item == "StonePack")
            myObject = GameManager.instance.SpawnStonePack();
        else if (item == "WoodPack")
            myObject = GameManager.instance.SpawnWoodPack();
        else if (item == "Seed")
            myObject = GameManager.instance.SpawnSeed();
        else if (item.Contains("Boost"))
            myObject = GameManager.instance.SpawnBoost(item);
        else if (item.Contains("Axe"))
            myObject = GameManager.instance.SpawnAxe();
        else
            myObject = null;

        return myObject;
    }
}




