
using UnityEngine;

// Different item anchors for the "Part" category
public enum EAnchorType
{
    Head,
    Core,
    Mobility
}

// Different item sizes for the "Part" category
public enum EModuleSize
{
    Small,
    Medium,
    Big
}

// Specific class for items in the "Part" category
public class Module : Item
{
    public EAnchorType Anchor { get; set; }
    public EModuleSize Size { get; set; }

    // Constructor
    public Module(string name, ERarityLevel rarity, Sprite image, EAnchorType anchor, EModuleSize size)
    {
        Name = name;
        Rarity = rarity;
        Image = image;
        Anchor = anchor;
        Size = size;
        itemType = EItemType.Module;
    }

    // Implementing the abstract method
    public override string GetItemDetails()
    {
        return $"{Name} (Rarity: {Rarity}) - Anchor: {Anchor}, Size: {Size}";
    }

    public override EItemType GetItemType()
    {
        return EItemType.Module;
    }
}
