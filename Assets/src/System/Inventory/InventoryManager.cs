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

    public void AddItemForPlayer<T>(int playerID, EItemType itemType, T item) where T : Item
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID].AddItem(itemType, item);
        }
    }

    public void RemoveItemForPlayer<T>(int playerID, EItemType itemType, T item) where T : Item
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            playerInventoryDict[playerID].RemoveItem(itemType, item);
        }
    }

    public bool HasItemForPlayer<T>(int playerID, EItemType itemType, T item) where T : Item
    {
        if (playerInventoryDict.ContainsKey(playerID))
        {
            return playerInventoryDict[playerID].HasItem(itemType, item);
        }

        return false;
    }
}
