using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemContainer<T> where T : Item
{
    private List<ItemSlot> itemSlots;
    private int maxCapacity;

    public ItemContainer(int maxCapacity = 0)
    {
        itemSlots = new List<ItemSlot>();
        this.maxCapacity = maxCapacity;
    }

    public void AddItem(Item item, int amount = 1)
    {
        var existingSlot = itemSlots.Find(slot => slot.Item.Name == item.Name);

        // Wenn das Item bereits existiert, nur die Menge erhöhen
        if (existingSlot != null)
        {
            existingSlot.Amount += amount;
        }
        // Wenn nicht, und die Kapazität es erlaubt, einen neuen Slot erstellen
        else
        {
            if (maxCapacity == 0 || itemSlots.Count < maxCapacity)
            {
                itemSlots.Add(new ItemSlot(item, amount));
            }
            else
            {
                Debug.Log("Item container is full!");
            }
        }
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        var existingSlot = itemSlots.Find(slot => slot.Item.Name == item.Name);

        if (existingSlot != null)
        {
            if (existingSlot.Amount > amount)
            {
                existingSlot.Amount -= amount;
            }
            else
            {
                itemSlots.Remove(existingSlot);
            }
        }
        else
        {
            Debug.Log("Item not found in container!");
        }
    }

    public bool HasItem(Item item)
    {
        return itemSlots.Exists(slot => slot.Item.Name == item.Name);
    }

    public int GetItemCount()
    {
        return itemSlots.Sum(slot => slot.Amount);
    }

    public int GetItemAmount(Item item)
    {
        var existingSlot = itemSlots.Find(slot => slot.Item.Name == item.Name);
        return existingSlot != null ? existingSlot.Amount : 0;
    }

    public List<ItemSlot> GetAllItems()
    {
        return itemSlots;
    }
}

public class ItemSlot
{
    public Item Item;
    public int Amount;

    public ItemSlot(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}
