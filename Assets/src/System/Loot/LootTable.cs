using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELootTableType
{
    TypeUnkown = -1,
    Chest,
    Monster,
    SpecialEvent,
    Boss,
    // Add more types as needed
}

[System.Serializable]
public class LootTable
{
    public string Name;
    public string type;
    public List<Pool> pools;

    public ELootTableType lootType;

    internal void register()
    {
        if(Enum.TryParse(typeof(ELootTableType), type, true, out var result))
        {
            lootType = (ELootTableType)result;
            PrecomputePools();
            //TODO: Register to global LootManager
            Debug.Log($"LootTable for {lootType.ToString()} registered.");
        }
        else
        {
            lootType = ELootTableType.TypeUnkown;
            Debug.LogWarning($"LootTable has invalid LootType '{type}'. LootTable will not be applied!");
        }  
        
    }

    void PrecomputePools()
    {
        foreach (var pool in pools)
        {
            var precomputedPool = new PrecomputedPool
            {
                rolls = pool.rolls,
                conditions = pool.conditions,
                precomputedEntries = new List<Entry>()
            };

            //float cumulativeWeight = 0;
            foreach (var entry in pool.entries)
            {
                Entry precomputedEntry = new Entry
                {
                    name = entry.name,
                    weight = entry.weight,
                    quantity = entry.quantity,
                };

                precomputedEntry.registerEntry();
                if(precomputedEntry.itemType != EItemType.none)
                {
                    precomputedPool.precomputedEntries.Add(precomputedEntry);
                }                  
            }

            //After all Entriers are registered calculate the float-Lits for the weight-ranges
            precomputedPool.PrecomputeWeights();
            pool.ReferencePool(precomputedPool);

        }
    }
}

[System.Serializable]
public class Pool
{
    public string name;
    public int rolls;
    public List<Entry> entries;
    public List<Condition> conditions;
    public PrecomputedPool referringPreComputedPool;

    public void ReferencePool(PrecomputedPool pool)
    {
        referringPreComputedPool = pool;
    }
}

public enum EItemType
{
    none =-1,
    Module,
    Remains,
    Rune,
    //To be continues
}

[System.Serializable]
public class Entry
{
    public string name;  // "base:stone"
    public int weight = 1; // Standardgewicht, falls nicht definiert
    public int quantity = 1; // Standardanzahl, falls nicht definiert
    public EItemType itemType;
    public string instanceName;

    public void registerEntry()
    {
        var split = name.Split(':');
        if (split.Length != 2)
        {
            Debug.LogError($"Loot Entry ha invalid name {name}.");
            return;
        }
            
        
        if (Enum.TryParse(typeof(EItemType), split[0], true, out var result))
        {
            itemType = (EItemType)result;
        }
        else
        {
            itemType = EItemType.none;
            Debug.LogWarning($"Loot Entry has unknown ItemType '{split[0]}'. Entry will be ignored!");
        }

        instanceName = split[1];

        //TODO: Check if item exists

    }
}

[System.Serializable]
public class Condition
{
    public string condition;  // z.B. "random" oder "time"
    public float chance = 1.0f;  // Nur für "random"

}

public enum ETimeOfDay
{
    Day,
    Night
}

public enum EGameMode
{
    Easy,
    Medium,
    Hard
}
