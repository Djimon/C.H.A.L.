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
        
    }

    private void Start()
    {
        Initilize();
    }

    // Start is called before the first frame update
    void Initilize()
    {
        LootDeserializer deserializer = new LootDeserializer();
        lootTables = new List<LootTable>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("lootTables");
        Debug.Log($"{jsonFiles.Length} JSON files found in the lootTables folder.");

        foreach (var file in jsonFiles)
        {

            if (file == null)
            {
                Debug.LogWarning($"Failed to load resource at path Ressourcs/lootTables");
                continue;
            }

            //Debug.Log(file.text); //output the json-text

            LootTable lt = deserializer.LoadLootTable(file,itemRegistry);
            if (lt != null)
            {
                lootTables.Add(lt);
                //lt.register(); //eventually not neccasary, cine alreade registered during Deserialization
                Debug.Log($"regsitration of file {file.name} successfull.");
            }
            else
            {
                Debug.LogError($"Failed to deserialize JSON for file: {file.name}");
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

    public void RollLootTable(LootTable lootTable)
    {
        foreach (var pool in lootTable.pools)
        {
            if (CheckConditions(pool.conditions))
            {
                for (int i = 0; i < pool.rolls; i++)
                {
                    Entry selectedItem = GetRandomPrecomputedItemFromPool(pool.referringPreComputedPool);
                    Debug.Log($"looking for {selectedItem.name} ({selectedItem.instanceName}).");
                    ScriptableItemBase item = itemRegistry.GetItemByName(selectedItem.instanceName);
                    if (item != null)
                    {
                        Debug.Log($"Item Dropped: {selectedItem.name}, Quantity: {selectedItem.quantity}");
                        Debug.Log($"Item Details: {item.GetItemDetails()}");

                        //TODO: Add Item to inventory

                    }  

                    //TODO: translate Entry into real Items an add them to inventory
                }
            }
        }
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
        Debug.LogWarning($"Unknown condition type: {condType}");
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
        Debug.LogWarning($"Unknown time condition value: {time}");
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
