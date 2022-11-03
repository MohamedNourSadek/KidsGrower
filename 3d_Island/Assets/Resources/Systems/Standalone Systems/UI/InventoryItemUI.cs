using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI itemName;
    [SerializeField] public TextMeshProUGUI itemNumber;
    [SerializeField] public Button myButton;
    [SerializeField] public GameObject itemOptionsMenu;

    GameObject canvas;
    InventorySystem inventorySystem;

    public void Intialize(InventorySystem inventory, GameObject myCanvas, string name,int number, UnityAction onPress)
    {
        itemName.text = name;
        itemNumber.text = number.ToString();
        this.canvas = myCanvas;
        myButton.onClick.AddListener(onPress);
        this.inventorySystem = inventory;
    }
    public void OnPress()
    {
        var Obj = Instantiate(itemOptionsMenu,myButton.transform);
        Obj.GetComponent<OptionsPopUp>().Initialize(inventorySystem, this.itemName.text);
        Obj.transform.parent = canvas.transform;
    }
}
