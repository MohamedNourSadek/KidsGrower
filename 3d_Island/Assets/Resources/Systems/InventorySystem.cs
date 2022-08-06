using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    [SerializeField] public GameObject stockPile;
    public List<IInventoryItem> items = new();
    IController _myController;

    public InventorySystem(IController controller)
    {
        _myController = controller;
    }

    public void Add(IInventoryItem item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            item.GetGameObject().transform.position = new Vector3(2000, 2000, 2000);

            UIController.instance.RepeatMessage(
                item.GetGameObject().tag + " added to inventory",
                _myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));
        }
    }
    public void Remove(IInventoryItem item)
    {
        if (items.Contains(item))
            items.Remove(item);

        UIController.instance.RepeatMessage(
            item.GetGameObject().tag + " removed from inventory",
                _myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));
    }

    public void UpdateUI()
    {
        
    }

    public static bool IsStorable(Pickable _pickable)
    {
        if(_pickable.gameObject.GetComponent<IInventoryItem>() != null)
            return true;
        else
            return false;
    }
}



public interface IInventoryItem
{
    public GameObject GetGameObject();
}

