using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Progress;


public class LootDeserializer
{
    public LootTable LoadLootTable(TextAsset jsonFile, ItemRegistry itemRegistry)
    {
        // Deserialize into the raw structure
        RawLootTable rawLootTable = JsonUtility.FromJson<RawLootTable>(jsonFile.text);

        // Now convert it to the full LootTable
        LootTable lootTable = new LootTable
        {
            Name = jsonFile.name,
            type = rawLootTable.type,
            pools = new List<Pool>()
        };

        Debug.Log($"raw Type = {rawLootTable.type}, pools: {rawLootTable.pools.Count}");

        //lootTable.register();

        foreach (var rawPool in rawLootTable.pools)
        {
            Pool pool = new Pool
            {
                name = rawPool.name,
                rolls = rawPool.rolls,
                entries = new List<Entry>(),
                conditions = new List<Condition>()
            };

            // Process entries
            foreach (var rawEntry in rawPool.entries)
            {
                Entry entry = new Entry
                {
                    name = rawEntry.name,
                    weight = rawEntry.weight,
                    quantity = rawEntry.quantity
                };

                entry.registerEntry();  // Parse the item type based on name
                //TODO: ignore if registryEntry detects invlaid item or Type

                if(itemRegistry.IsItemKnown(entry.instanceName))
                {
                    pool.entries.Add(entry);
                }
                else
                {
                    Debug.LogWarning($"Item with name '{entry.instanceName}' doesn't exist.");
                    pool.entries.Add(new Entry { name = "missing_"+entry.instanceName, weight = 0, quantity = 0, itemType = EItemType.none, instanceName = "missing" });
                    //TODO: Save as scriptable Object in new path

                    CreateAndSaveMissingItemAsSO(entry);

                }

            }

            // Process conditions
            foreach (var rawCondition in rawPool.conditions)
            {
                Condition condition = new Condition
                {
                    condition = rawCondition.condition,
                    chance = rawCondition.chance
                };
                pool.conditions.Add(condition);
            }

            lootTable.pools.Add(pool);
        }

        // Parse the type (enum) after loading
        lootTable.register();  // Register type and preprocess pools

        // Now you can use the populated LootTable
        Debug.Log("LootTable successfully loaded and processed.");
        return lootTable;
    }

    private void CreateAndSaveMissingItemAsSO(Entry entry)
    {
        EItemType itemType = entry.itemType;
        string instanceName = entry.instanceName;
        string folder = itemType switch 
        {
            EItemType.none => "missingType",
            EItemType.Module => "Module",
            EItemType.Remains => "Remains",
            EItemType.Rune => "Rune",
            _ => "missingType"
        };
        //Path.Combine(Application.dataPath, $"Resources/lootTables/{lootTablename}.json");
        string path = Path.Combine(Application.dataPath, $"Resources/Items/{folder}/_missing/");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string filename = $"{instanceName}.asset";
        string assetPath = Path.Combine(path, filename);
        string relativePath = "Assets" + assetPath.Replace(Application.dataPath, "");

        //Create and Save the ScriptableObject
        ScriptableTemplate asset = ScriptableObject.CreateInstance<ScriptableTemplate>();
        asset.itemName = entry.name;
        asset.rarity = ERarityLevel.Common;
        asset.name = entry.instanceName;

        AssetDatabase.CreateAsset(asset, relativePath);
        AssetDatabase.SaveAssets();

        Debug.Log($"ScriptableObject for missing item '{instanceName}' saved under Ressources/items/{folder}/_missing/.");
    }
}

[System.Serializable]
public class RawLootTable
{
    public string type;  // Keep as string to process later
    public List<RawPool> pools;
}

[System.Serializable]
public class RawPool
{
    public string name;
    public int rolls;
    public List<RawEntry> entries;
    public List<RawCondition> conditions;
}

[System.Serializable]
public class RawEntry
{
    public string name;
    public int weight = 1;  // Default value if not present
    public int quantity;  // Default value if not present
}

[System.Serializable]
public class RawCondition
{
    public string condition;
    public float chance = 1.0f;  // Default to 1 if not present
}