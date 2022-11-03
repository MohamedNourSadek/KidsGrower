using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPopUp : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Button drop;
    [SerializeField] Button hand;

    InventorySystem inventorySystem;
    string item;

    public void Initialize(InventorySystem inventory, string item)
    {
        this.inventorySystem = inventory;
        this.item = item;
    }

    private void Awake()
    {
        drop.Select();
        drop.onClick.AddListener(OnDropPressed);
        hand.onClick.AddListener(OnHandPressed);
    }
    private void Update()
    {
        if (IsAnySelected() == false || inventorySystem.Exists(item) == false)
            Destroy(this.gameObject);
    }
    public bool IsAnySelected()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if(current == background.gameObject || current == drop.gameObject || current == hand.gameObject)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
    void OnDropPressed()
    {
        inventorySystem.Remove(item);
        UIGame.instance.DisplayInventory(true, inventorySystem);
    }
    void OnHandPressed()
    {
        if(PlayerSystem.instance.handSystem.GetObjectInHand() != null)
        {
            Pickable inHand = PlayerSystem.instance.handSystem.GetObjectInHand();
            
            PlayerSystem.instance.handSystem.DropObjectInHand();

            if (InventorySystem.IsStorable(inHand))
            {
                inventorySystem.Add(inHand.gameObject, true);
            }
        }

        Pickable newItem = inventorySystem.Remove(item).GetComponent<Pickable>();
        PlayerSystem.instance.handSystem.PickObject(newItem);
        UIGame.instance.DisplayInventory(true, inventorySystem);
    }
}
