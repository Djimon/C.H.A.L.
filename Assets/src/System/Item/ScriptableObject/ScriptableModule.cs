using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Parts")]
public class ScriptableParts : ScriptableItemBase
{
    public EAnchorType anchor;
    public ESizeType size;

    // Override to provide item-specific details
    public override string GetItemDetails()
    {
        return $"{itemName} (Rarity: {rarity}) - Anchor: {anchor}, Size: {size}";
    }
}
