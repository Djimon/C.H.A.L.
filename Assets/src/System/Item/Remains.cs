using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Specific class for items in the "PowerUp" category
public class Remains : Item
{
    public int sourceMonsterID;
    public int DNAValue = 0;
    public float SellValue = 0; //how much Gold you get for this
    public float ExchangeCost = 1; //cost to exchange other Remains to this.

    // Constructor
    public Remains(string name, ERarityLevel rarity, Sprite image, int monsterID, int dnaValue = 10, float sellValue=0, float exchangeCost=1)
    {
        Name = name;
        Rarity = rarity;
        Image = image;
        sourceMonsterID = monsterID;
        DNAValue = dnaValue;
        SellValue = sellValue;
        ExchangeCost = exchangeCost;   
        itemType = EItemType.Remains;
    }

    // Implementing the abstract method
    public override string GetItemDetails()
    {
        return $"{Name} (Rarity: {Rarity}) - from: {sourceMonsterID}, Value: {SellValue}, ExchangeCost: {ExchangeCost}";
    }

    public override EItemType GetItemType()
    {
        return EItemType.Remains;
    }
}
