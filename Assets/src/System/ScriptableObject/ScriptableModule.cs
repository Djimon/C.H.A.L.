using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Parts")]
public class ScriptableModule : ScriptableItemBase
{
    public EAnchorType anchor;
    public EModuleSize size;

    public override Item CreateInstance()
    {
        return new Module(name, rarity, image,anchor, size);
    }

    // Override to provide item-specific details
    public override string GetItemDetails()
    {
        return $"{itemName} (Rarity: {rarity}) - Anchor: {anchor}, Size: {size}";
    }
}
