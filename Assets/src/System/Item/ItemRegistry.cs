using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemRegistry : MonoBehaviour
{
    [SerializeField]
    public Dictionary<string, ScriptableItemBase> itemDictionary;

    private void Awake()
    {

        // Initialize the dictionary for faster lookup
        itemDictionary = new Dictionary<string, ScriptableItemBase>();
        string rootFolder = "Items";

        LoadItemsRecursively(rootFolder);

        DebugManager.Log($"Loaded {itemDictionary.Count} valid items from the Items folder.");
    }

    private void LoadItemsRecursively(string folder)
    {
        ScriptableObject[] allItems = Resources.LoadAll<ScriptableObject>(folder);

        foreach (var item in allItems)
        {
            if (item is ScriptableItemBase scriptableItem)
            {
                if (itemDictionary.ContainsKey(scriptableItem.itemName))
                {
                    DebugManager.Warning($"Item with name '{scriptableItem.itemName}' already exists. skipping duplicates.", 2, "Items");
                    continue;
                }
                itemDictionary.Add(scriptableItem.itemName, scriptableItem);
                DebugManager.Log($"Item '{scriptableItem.itemName}' loaded");
            }
        }
        string fullpath = Path.Combine(Application.dataPath, "Resources", folder);

        string[] subfolders = Directory.GetDirectories(fullpath);


        foreach (var subfolder in subfolders)
        {
            string foldername = Path.GetFileName(subfolder);
            string realtivePath = Path.Combine(fullpath, foldername);
            LoadItemsRecursively (realtivePath);
        }
    }

    public ScriptableItemBase GetItemByName(string itemName)
    {
        // Look up the item in the dictionary and return it
        if (itemDictionary.TryGetValue(itemName, out var item))
        {
            DebugManager.Log($"item {item.itemName} found in Itemregistry", 3, "Items");
            return item;
        }

        DebugManager.Warning($"Item with name '{itemName}' not found in registry.", 1, "Items");
        return null;
    }

    public bool IsItemKnown(string itemName)
    {
        return itemDictionary.ContainsKey(itemName);
    }
        
}