using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InventorySystem
{
    List<IInventoryItem> items = new();
    IController myController;
    string tempTag = "item";

    public InventorySystem(IController controller)
    {
        myController = controller;
    }
    public void Add(IInventoryItem item)
    {
        if (!items.Contains(item))
        {
            if (items.Count == 0)
                UIGame.instance.CreateInventoryUI(tempTag, OnButtonClick);

            items.Add(item);

            item.GetGameObject().transform.position = new Vector3(2000, 2000, 2000);

            UIGame.instance.ShowRepeatingMessage(
                item.GetGameObject().tag + " added to inventory",
                myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));
        }

        UpdateUI();
    }
    public void Remove(IInventoryItem item)
    {
        if (items.Contains(item))
            items.Remove(item);

        UIGame.instance.ShowRepeatingMessage(
            item.GetGameObject().tag + " removed from inventory",
                myController.GetBody().transform, 0.5f, 1, new ConditionChecker(true));

        item.GetGameObject().transform.position = myController.GetBody().transform.position;

        UpdateUI();
    }
    public void OnButtonClick()
    {
        if (items.Count >= 0)
            Remove(items[items.Count - 1]);

    }
    public static bool IsStorable(Pickable pickable)
    {
        if(pickable.gameObject.GetComponent<IInventoryItem>() != null)
            return true;
        else
            return false;
    }



    void UpdateUI()
    {
        if (items.Count == 0)
            UIGame.instance.DestroyInventoryUI(tempTag);
        else if(items.Count > 0)
            UIGame.instance.UpdateInventoryUI(tempTag, items.Count);
    }
}




