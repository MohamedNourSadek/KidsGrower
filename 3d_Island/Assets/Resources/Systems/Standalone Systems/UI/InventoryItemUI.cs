using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI itemName;
    [SerializeField] public TextMeshProUGUI itemNumber;
    [SerializeField] public Button myButton;

    InventorySystem inventorySystem;
    
    public void Intialize(InventorySystem inventorySystem, string name,int number, UnityEngine.Events.UnityAction onPress)
    {
        itemName.text = name;
        itemNumber.text = number.ToString();
        this.inventorySystem = inventorySystem;
        myButton.onClick.AddListener(onPress);
    }

    public void OnPress()
    {
        foreach(var item in inventorySystem.GetItems())
        {
            if(item.GetGameObject().tag == itemName.text)
            {
                inventorySystem.Remove(item);
                UIGame.instance.DisplayInventory(true, inventorySystem);
                break;
            }    
        }
    }

}
