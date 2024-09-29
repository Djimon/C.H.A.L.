using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer<T> where T : Item
{
    private List<T> items = new List<T>();
    private int maxCapacity;

    public ItemContainer(int maxCapacity = 0)
    {
        this.maxCapacity = maxCapacity;
    }

    public void AddItem(T item)
    {
        if (maxCapacity == 0 || items.Count < maxCapacity)
        {
            items.Add(item);
        }
        else
        {
            Debug.Log("Item container is full!");
        }
    }

    public void RemoveItem(T item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
        else
        {
            Debug.Log("Item not found in container!");
        }
    }

    public bool HasItem(T item)
    {
        return items.Contains(item);
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    public List<T> GetAllItems()
    {
        return new List<T>(items);
    }
}
