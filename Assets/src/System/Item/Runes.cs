using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : Item
{
    public int runeID {  get; private set; }
    public string Element { get; set; }  // e.g., Fire, Ice, Lightning
    public int Power { get; set; }

    public Rune(string name, ERarityLevel rarity, Sprite image, string element, int power)
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
