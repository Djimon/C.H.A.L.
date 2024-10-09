using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class PlayerInventory
{
    private ItemContainer itemContainer;


    public PlayerInventory()
    {
        itemContainer = new ItemContainer();
    }

    // Methode zum Hinzufügen eines Items
    public void AddItem(Item item, int amount = 1)
    {
        itemContainer.AddItem(item, amount);
    }

    // Methode zum Entfernen eines Items
    public void RemoveItem(Item item, int amount = 1)
    {
        itemContainer.RemoveItem(item, amount);
    }

    public void RemoveItem(string itemName, int amount = 1)
    {
        itemContainer.RemoveItem(itemName, amount);
    }


    // Methode zum Überprüfen, ob ein bestimmtes Item vorhanden ist
    public bool HasItem(Item item)
    {
        return itemContainer.HasItem(item);
    }

    // Methode, um Items eines bestimmten Typs zu filtern (z.B. Remains)
    public List<Item> GetItemsByType(EItemType type) 
    {
        return itemContainer.GetAllItems()
                            .Where(slot => slot.Item.itemType == type)
                            .Select(slot => slot.Item as Item)
                            .ToList();
    }

    public List<ItemSlot> GetItemsByTypeWithAmount(EItemType type)
    {
        return itemContainer.GetAllItems()
                            .Where(slot => slot.Item.itemType == type)  // Filtert nach EItemType
                            .ToList();  // Gibt eine Liste von ItemSlots zurück
    }

    // Gesamtzahl eines bestimmten Items holen
    public int GetItemCount(string itemName)
    {
        return itemContainer.GetItemAmount(itemName);
    }

}