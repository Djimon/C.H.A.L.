using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class PlayerInventory
{
    //private Dictionary<EItemType, object> itemContainers = new Dictionary<EItemType, object>();
    private ItemContainer<Module> modulesContainer;
    private ItemContainer<Remains> remainsContainer;
    private ItemContainer<Rune> runesContainer;

    public PlayerInventory()
    {
        // Hier können spezifische Kapazitätswerte pro Itemtyp festgelegt werden, wenn gewünscht
        modulesContainer = new ItemContainer<Module>();   // Max. 10 Module
        remainsContainer = new ItemContainer<Remains>();   // Keine Begrenzung
        runesContainer = new ItemContainer<Rune>(30); //= 30 max known runes
    }


    public void AddItem<T>(EItemType itemType, T item) where T : Item
    {
        switch (itemType)
        {
            case EItemType.Module: modulesContainer.AddItem(item);
                    break;
            case EItemType.Remains: remainsContainer.AddItem(item); 
                break;
            case EItemType.Rune: runesContainer.AddItem(item);
                break;
            default: DebugManager.Warning($"Container for type {itemType} does not exists.", 2, "Items");
                break;
        }

    }

    public void RemoveItem<T>(EItemType itemType, T item) where T : Item
    {
        switch (itemType)
        {
            case EItemType.Module:
                modulesContainer.RemoveItem(item);
                break;
            case EItemType.Remains:
                remainsContainer.RemoveItem(item);
                break;
            case EItemType.Rune:
                runesContainer.RemoveItem(item);
                break;
            default:
                DebugManager.Warning($"Container for type {itemType} does not exists.", 2, "Items");
                break;
        }

    }

    public bool HasItem<T>(EItemType itemType, T item) where T : Item
    {
        switch (itemType)
        {
            case EItemType.Module:
                return modulesContainer != null && modulesContainer.HasItem(item);
            case EItemType.Remains:
                return remainsContainer != null && remainsContainer.HasItem(item);
            case EItemType.Rune:
                return runesContainer != null && runesContainer.HasItem(item);
            default:
                DebugManager.Warning($"Container for type {itemType} does not exists.", 2, "Items");
                return false;
        }

    }

    public int GetItemCount(string itemName)
    {
        int count = modulesContainer.GetItemAmount(itemName);

        if (count == 0)
        {
            count = remainsContainer.GetItemAmount(itemName);
        }

        if (count == 0)
        {
            count = runesContainer.GetItemAmount(itemName);
        }

        return count;
    }

    internal void RemoveItem(string itemName)
    {
        modulesContainer.RemoveItem(itemName);
        remainsContainer.RemoveItem(itemName);
        runesContainer.RemoveItem(itemName);
    }

    public ItemContainer<T> GetContainer<T>(EItemType itemType) where T : Item
    {
        switch (itemType)
        {
            case EItemType.Module:
                return modulesContainer as ItemContainer<T>;
            case EItemType.Remains:
                return remainsContainer as ItemContainer<T>;
            case EItemType.Rune:
                return runesContainer as ItemContainer<T>; ;
            default:
                DebugManager.Warning($"Container for type {itemType} does not exists.", 2, "Items");
                return null;
        }
    }

    
}