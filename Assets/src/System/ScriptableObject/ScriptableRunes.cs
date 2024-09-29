using UnityEngine;

[CreateAssetMenu(fileName = "NewRune", menuName = "Items/Rune")]
public class ScriptableRune : ScriptableItemBase
{
    public string effectText;
    public float value;
    public float duration;

    // Override to provide PowerUp-specific details
    public override string GetItemDetails()
    {
        return $"{itemName} (Rarity: {rarity}) - Effect: {effectText}, Value: {value}, Duration: {duration} seconds";
    }
}
