using UnityEngine;

[CreateAssetMenu(fileName = "NewRemains", menuName = "Items/Remains")]
public class ScriptableRemains : ScriptableItemBase
{
    public int sourceMonsterID;
    public int dnaValue;
    public float SellValue = 0; //how much Gold you get for this
    public float ExchangeCost = 1; //cost to exchange other Remains to this.

    public override Item CreateInstance()
    {
        return new Remains(name, rarity,image, sourceMonsterID, dnaValue, SellValue, ExchangeCost);
    }


    // Override to provide PowerUp-specific details
    public override string GetItemDetails()
    {
        return $"{itemName} (Rarity: {rarity}) - Sell value: {SellValue} Gold, ExchangeCost: {ExchangeCost}";
    }
}