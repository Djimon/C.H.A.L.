using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class LootSystem : MonoBehaviour
{
    public ItemRegistry itemRegistry;
    public List<LootTable> lootTables;

    private void Awake()
    {
        itemRegistry = GetComponent<ItemRegistry>();
    }

    private void Start()
    {
        Initilize();
    }

    public void AddLootFromMobKill(int playerID, EMonsterType victimMonsterType)
    {
        //TODO: implement
        //LootTable table = FindLootTable(victimMonsterType);
        //List<Item> loot = RollLootTable(table);
        // TODO:
        // Add Items do PlayerInventory
        DebugManager.Log($"Player {playerID} gets loot from killing {victimMonsterType}", 2, "Info", Color.green);

    }

    private LootTable FindLootTable(EMonsterType victimMonsterType)
    {
        //TODO: implement
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Initilize()
    {
        LootDeserializer deserializer = new LootDeserializer();
        lootTables = new List<LootTable>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("lootTables");
        DebugManager.Log($"{jsonFiles.Length} JSON files found in the lootTables folder.");

        foreach (var file in jsonFiles)
        {

            if (file == null)
            {
                DebugManager.Warning($"Failed to load resource at path Ressourcs/lootTables",2,"Items");
                continue;
            }

            //DebugManager.Log(file.text); //output the json-text

            LootTable lt = deserializer.LoadLootTable(file,itemRegistry);
            if (lt != null)
            {
                lootTables.Add(lt);
                //lt.register(); //eventually not neccasary, cine alreade registered during Deserialization
                DebugManager.Log($"regsitration of file {file.name} successfull.");
            }
            else
            {
                DebugManager.Error($"Failed to deserialize JSON for file: {file.name}",2,"Items");
            }                     
        }    
    }

    Entry GetRandomPrecomputedItemFromPool(PrecomputedPool pool)
    {
        float randValue = UnityEngine.Random.Range(0, pool.totalWeights);

        // Schnelles Auffinden mittels Binary Search
        int index = pool.cumulativeWeights.BinarySearch(randValue);

        // Falls BinarySearch keinen genauen Treffer findet, liefert es einen negativen Index zurück
        if (index < 0)
        {
            index = ~index;  // Bitweise Umkehrung, um den nächsten Bereich zu finden
        }

        return pool.precomputedEntries[index];
    }

    public List<Item> RollLootTable(LootTable lootTable)
    {
        List<Item> loot = new List<Item>();

        foreach (var pool in lootTable.pools)
        {
            if (CheckConditions(pool.conditions))
            {
                for (int i = 0; i < pool.rolls; i++)
                {
                    Entry selectedItem = GetRandomPrecomputedItemFromPool(pool.referringPreComputedPool);
                    DebugManager.Log($"looking for {selectedItem.name} ({selectedItem.instanceName}).");
                    ScriptableItemBase item = itemRegistry.GetItemByName(selectedItem.instanceName);
                    if (item != null)
                    {
                        DebugManager.Log($"Item Dropped: {selectedItem.name}, Quantity: {selectedItem.quantity}");
                        DebugManager.Log($"Item Details: {item.GetItemDetails()}");

                        //TODO: Add Item to inventory
                        //TODO: translate Entry into real Items an add them to inventory

                        //loot.Add(item);

                    }   
                }
            }
        }

        return loot;
    }

    private bool CheckConditions(List<Condition> conditions)
    {
        //EvaluateCondition: If one is false, no further evaluation is done
        return conditions.All(condition => EvaluateCondition(condition));
    }

    private bool EvaluateCondition(Condition condition)
    {
        string[] conditionParts = condition.condition.Split(':');
        string condType = conditionParts[0];
        string condValue = conditionParts.Length > 1 ? conditionParts[1] : "";

        return condType switch
        {
            "random" => UnityEngine.Random.Range(0f, 1f) < condition.chance,
            "time" => CheckTimeCondition(condValue),
            "mode" => CheckModeCondition(condValue),
            _ => LogUnknownCondition(condType)
        };

    }

    private bool LogUnknownCondition(string condType)
    {
        DebugManager.Warning($"Unknown condition type: {condType}",2,"Items");
        return false;
    }

    private bool CheckModeCondition(string mode)
    {
        // Assuming you have a way to track the current game mode
        string currentMode = GetCurrentGameMode(); // This method returns "easy", "medium", or "hard"

        return currentMode.Equals(mode, StringComparison.OrdinalIgnoreCase);
    }

    private bool CheckTimeCondition(string time)
    {
        // Assuming you have a method or a variable that tracks the time of day.
        // Here, we simulate it as a boolean, but you could replace it with your actual game time system.
        bool isNight = IsNightTime(); // This is your own method

        return time switch
        {
            "day" => !isNight,
            "night" => isNight,
            _ => LogUnknownTimeCondition(time)
        };
    }

    private bool LogUnknownTimeCondition(string time)
    {
        DebugManager.Warning($"Unknown time condition value: {time}", 2, "Items");
        return false;
    }

    private string GetCurrentGameMode()
    {
        // Replace this with your actual game mode logic
        // For example, you might have a global game manager that tracks the current difficulty.
        //return GameManager.Instance.CurrentDifficulty.ToString().ToLower(); // Returns "easy", "medium", or "hard"
        return "easy";
    }

    private bool IsNightTime()
    {
        // Placeholder logic, replace with your actual day/night system
        return false;
    }


    public List<LootTable> GetLootTablesByType(ELootTableType lootType)
    {
        var result = lootTables.Where(lt => lt.lootType == lootType).ToList();
        return result;
    }

    public Dictionary<ELootTableType, List<LootTable>> GetAllLootTablesGroupedByType()
    {
        return lootTables
            .GroupBy(lt => lt.lootType)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

}
