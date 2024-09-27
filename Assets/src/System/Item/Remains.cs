using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Specific class for items in the "PowerUp" category
public class Remains : ItemBase
{
    public string Text { get; set; }
    public float Value { get; set; }
    public float Time { get; set; }

    // Constructor
    public Remains(string name, ERarityLevel rarity, Sprite image, string text, float value, float time)
    {
        Name = name;
        Rarity = rarity;
        Image = image;
        Text = text;
        Value = value;
        Time = time;
    }

    // Implementing the abstract method
    public override string GetItemDetails()
    {
        return $"{Name} (Rarity: {Rarity}) - Effect: {Text}, Value: {Value}, Time: {Time} seconds";
    }
}
