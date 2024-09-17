using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPowerUpType
{
    Speed,
    Luck,
    Damage
}

// Specific class for items in the "PowerUp" category
public class PowerUp : ItemBase
{
    public string Text { get; set; }
    public float Value { get; set; }
    public float Time { get; set; }

    public EPowerUpType PowerUpType { get; set; }

    // Constructor
    public PowerUp(string name, ERarityLevel rarity, Sprite image, EPowerUpType type, string text, float value, float time)
    {
        Name = name;
        Rarity = rarity;
        Image = image;
        PowerUpType = type;
        Text = text;
        Value = value;
        Time = time;
    }

    // Implementing the abstract method
    public override string GetItemDetails()
    {
        return $"{Name} (Rarity: {Rarity}) - Effect: {Text}, Type: {PowerUpType}, Value: {Value}, Time: {Time} seconds";
    }
}
