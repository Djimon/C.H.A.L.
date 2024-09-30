using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
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
        }
    }

    public void RemoveItemForPlayer(int playerID, Item item) 
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID].RemoveItem(item.GetItemType(), item);
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

    internal void AddItemsForPlayer(int teamNumber, List<Item> items)
    {
        
        foreach (Item item in items)
        {
            AddItemForPlayer(teamNumber, item);
        }
    }

}
