using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : ItemBase
{
    public string Element { get; set; }  // e.g., Fire, Ice, Lightning
    public int Power { get; set; }

    public Rune(string name, RarityLevel rarity, string image, string element, int power)
    {
        Name = name;
        Rarity = rarity;
        Image = image;
        Element = element;
        Power = power;
    }

    public override string GetItemDetails()
    {
        return $"{Name} (Rarity: {Rarity}) - Element: {Element}, Power: {Power}";
    }
}
