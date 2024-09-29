using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private Dictionary<EItemType, object> itemContainers = new Dictionary<EItemType, object>();

    public PlayerInventory()
    {
        // Hier können spezifische Kapazitätswerte pro Itemtyp festgelegt werden, wenn gewünscht
        itemContainers[EItemType.Module] = new ItemContainer<Module>();   // Max. 10 Module
        itemContainers[EItemType.Remains] = new ItemContainer<Remains>();   // Keine Begrenzung
        itemContainers[EItemType.Rune] = new ItemContainer<Rune>();        // Nur eine Rune
    }

    public void AddItem<T>(EItemType itemType, T item) where T : Item
    {
        if (itemContainers.ContainsKey(itemType))
        {
            var container = itemContainers[itemType] as ItemContainer<T>;
            container?.AddItem(item);
        }
    }

    public void RemoveItem<T>(EItemType itemType, T item) where T : Item
    {
        if (itemContainers.ContainsKey(itemType))
        {
            var container = itemContainers[itemType] as ItemContainer<T>;
            container?.RemoveItem(item);
        }
    }

    public bool HasItem<T>(EItemType itemType, T item) where T : Item
    {
        if (itemContainers.ContainsKey(itemType))
        {
            var container = itemContainers[itemType] as ItemContainer<T>;
            return container != null && container.HasItem(item);
        }

        return false;
    }
}