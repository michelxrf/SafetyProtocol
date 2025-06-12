using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private List<Item> itemList = new();

    private void Awake()
    {
        foreach (Item item in itemList)
        {
            item.model.SetActive(item.isEquipped);
        }
    }

    public void ReverseEquipment()
    {
        // unequip all that's equiped and equip all that's not

        foreach (Item equipment in itemList)
        {
            equipment.isEquipped = !equipment.isEquipped;
            equipment.model.SetActive(equipment.isEquipped);
        }
    }

    public void FullyEquip(bool newState)
    {
        // used to fully equip a worker

        foreach (Item equipment in itemList)
        {
            equipment.isEquipped = newState;
            equipment.model.SetActive(newState);
        }
    }

    public void EquipItem(string itemName, bool newState)
    {
        // used to equip an item to the worker and update the model's visibility

        Item foundEquipment = itemList.Find(e => e.equipmentName == itemName);

        if (foundEquipment != null)
        {
            Debug.LogError($"trying to equip a {itemName} failed, verify if the name is correct.");
            return;
        }

        foundEquipment.isEquipped = newState;
        foundEquipment.model.SetActive(newState);
    }
}

[Serializable]
public class Item
{
    public string equipmentName;
    public GameObject model;
    public bool isEquipped;
}