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

        DebugManager.Log($"raw Type = {rawLootTable.type}, pools: {rawLootTable.pools.Count}");

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
                    DebugManager.Warning($"Item with name '{entry.instanceName}' doesn't exist.", 2, "Items");
                    pool.entries.Add(new Entry { name = entry.instanceName, weight = 0, quantity = 0, itemType = EItemType.none, instanceName = "missing" });

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
        DebugManager.Log("LootTable successfully loaded and processed.");
        return lootTable;
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