using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryHelper[] pseudoPlayerInventoryDict;
    private Dictionary<int, PlayerInventory> playerInventoryDict = new Dictionary<int, PlayerInventory>();

    private void Awake()
    {
        // Initialisierungen können hier vorgenommen werden.
    }

    void Start()
    {
        // Initiale Logik kann hier implementiert werden.
    }

    void Update()
    {
        // Update-Logik kann hier implementiert werden.
    }

    public PlayerInventory GetPlayerinventory(int playerID)
    {
        return playerInventoryDict[playerID];
    }

    private void UpdateInventoryHelper()
    {
        pseudoPlayerInventoryDict = new InventoryHelper[playerInventoryDict.Count];
        int index = 0;
        foreach(var kvp in playerInventoryDict)
        {
            pseudoPlayerInventoryDict[index] = new InventoryHelper 
            { 
                playerID = kvp.Key,
                inventory = kvp.Value
            };
            index++;
        }
    }

    public void CreatePlayerInventory(int playerID)
    {
        if (!playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID] = new PlayerInventory();
        }
    }

    public void AddItemForPlayer(int playerID, Item item)
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID].AddItem(item.GetItemType(), item);
            DebugManager.Log($"{item.Name} was added to Player {playerID}", 3, "Items");
            UpdateInventoryHelper();
        }
    }

    public void RemoveItemForPlayer(int playerID, Item item) 
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID].RemoveItem(item.GetItemType(), item);
            UpdateInventoryHelper();
        }
    }

    public bool HasItemForPlayer(int playerID, Item item) 
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            return playerInventoryDict[playerID].HasItem(item.GetItemType(), item);
        }

        return false;
    }

    public int GetItemAmountForPlayer(int playerID, string itemName)
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID].GetItemCount(itemName);
        }

        return 0;
    }

    internal void AddItemsForPlayer(int teamNumber, List<Item> items)
    {
        
        foreach (Item item in items)
        {
            AddItemForPlayer(teamNumber, item);
        }
    }

}

[System.Serializable]
public struct InventoryHelper
{
    public int playerID;
    public PlayerInventory inventory;
}
