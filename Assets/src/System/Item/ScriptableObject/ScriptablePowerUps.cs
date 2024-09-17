using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "Items/PowerUp")]
public class ScriptablePowerUp : ScriptableItemBase
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