using System.Collections.Generic;
using UnityEngine;

// Base class for all item categories
public abstract class Item
{
    public string Name { get; set; }
    public Sprite Image { get; set; } // Could be a path or sprite reference
    public ERarityLevel Rarity { get; set; }

    public int globalItemID;

    // Abstract method to get detailed info about the item
    public abstract string GetItemDetails();

    public abstract EItemType GetItemType();
}

// Enumeration for rarity levels
public enum ERarityLevel
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}


// Example usage in your loot system
//public class LootSystemExample : MonoBehaviour
//{
//    private void Start()
//    {
//        // Example of creating an "Item"
//        Item helmet = new Item("Iron Helmet", ERarityLevel.Rare, "helmet.png", EAnchorType.Head, ESizeType.Medium);
//        DebugManager.Log(helmet.GetItemDetails());

//        // Example of creating a "PowerUp"
//        PowerUp strengthBoost = new PowerUp("Strength Boost", ERarityLevel.Epic, "powerup.png", "Increases strength", 20f, 30f);
//        DebugManager.Log(strengthBoost.GetItemDetails());
//    }
//}