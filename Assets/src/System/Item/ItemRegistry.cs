using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemRegistry : MonoBehaviour
{
    private Dictionary<string, ScriptableItemBase> itemDictionary;

    private void Awake()
    {
        ScriptableItemBase[] allItems = Resources.LoadAll<ScriptableItemBase>("Items");
        // Initialize the dictionary for faster lookup
        itemDictionary = new Dictionary<string, ScriptableItemBase>();
        foreach (var item in allItems)
        {
            if(itemDictionary.ContainsKey(item.itemName))
            {
                Debug.LogWarning($"Item with name '{item.itemName}' already exists. skipping duplicates.");
                continue;
            }
            itemDictionary.Add(item.itemName, item);
            Debug.Log($"Item '{item.itemName}' loaded");
        }

        Debug.Log($"Loaded {allItems.Length} items from the Items folder.");
    }

    public ScriptableItemBase GetItemByName(string itemName)
    {
        // Look up the item in the dictionary and return it
        if (itemDictionary.TryGetValue(itemName, out var item))
        {
            return item;
        }

        Debug.LogWarning($"Item with name '{itemName}' not found in registry.");
        return null;
    }

    public bool IsItemKnown(string itemName)
    {
        return itemDictionary.ContainsKey(itemName);
    }
        
}