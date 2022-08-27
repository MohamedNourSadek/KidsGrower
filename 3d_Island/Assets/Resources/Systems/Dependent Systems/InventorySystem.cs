using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    List<IInventoryItem> items = new();
    IController myController;
    string tempTag = "item";

    public InventorySystem(IController _controller)
    {
        myController = _controller;
    }
    public void Add(IInventoryItem _item)
    {
        if (!items.Contains(_item))
        {
            if (items.Count == 0)
                UIController.instance.CreateInventoryUI(tempTag, OnButtonClick);

            items.Add(_item);

            _item.GetGameObject().transform.position = new Vector3(2000, 2000, 2000);

            UIController.instance.RepeatMessage(
                _item.GetGameObject().tag + " added to inventory",
                myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));
        }

        UpdateUI();
    }
    public void Remove(IInventoryItem _item)
    {
        if (items.Contains(_item))
            items.Remove(_item);

        UIController.instance.RepeatMessage(
            _item.GetGameObject().tag + " removed from inventory",
                myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));

        _item.GetGameObject().transform.position = myController.GetBody().transform.position;

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
            UIController.instance.DestroyInventoryUI(tempTag);
        else if(items.Count > 0)
            UIController.instance.UpdateInventoryUI(tempTag, items.Count);
    }
}




