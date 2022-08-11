using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    [SerializeField] public GameObject stockPile;
    public List<IInventoryItem> items = new();
    IController _myController;

    string _tempTag = "item";

    public InventorySystem(IController controller)
    {
        _myController = controller;
    }

    public void Add(IInventoryItem item)
    {
        if (!items.Contains(item))
        {
            if (items.Count == 0)
                UIController.instance.CreateInventoryUI(_tempTag, OnButtonClick);

            items.Add(item);
            item.GetGameObject().transform.position = new Vector3(2000, 2000, 2000);

            UIController.instance.RepeatMessage(
                item.GetGameObject().tag + " added to inventory",
                _myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));
        }

        UpdateUI();
    }
    public void Remove(IInventoryItem item)
    {
        if (items.Contains(item))
            items.Remove(item);

        UIController.instance.RepeatMessage(
            item.GetGameObject().tag + " removed from inventory",
                _myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));

        item.GetGameObject().transform.position = _myController.GetBody().transform.position;

        UpdateUI();
    }
    public void OnButtonClick()
    {
        if (items.Count >= 0)
            Remove(items[items.Count - 1]);

    }
    public static bool IsStorable(Pickable _pickable)
    {
        if(_pickable.gameObject.GetComponent<IInventoryItem>() != null)
            return true;
        else
            return false;
    }

    void UpdateUI()
    {
        if (items.Count == 0)
            UIController.instance.DestroyInventoryUI(_tempTag);
        else if(items.Count > 0)
            UIController.instance.UpdateInventoryUI(_tempTag, items.Count);
    }
}



public interface IInventoryItem
{
    public GameObject GetGameObject();
}
