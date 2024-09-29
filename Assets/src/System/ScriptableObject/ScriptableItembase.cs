using UnityEngine;

// Base class for all items as ScriptableObjects
public abstract class ScriptableItemBase : ScriptableObject
{
    public string itemName;
    public Sprite image;  // Could also be a string if you're using paths
    public ERarityLevel rarity;

    // Abstract method for getting details specific to the item
    public abstract string GetItemDetails();
}