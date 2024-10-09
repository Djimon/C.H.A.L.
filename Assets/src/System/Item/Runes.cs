using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : Item
{
    public int RuneID {  get; private set; }
    public ERuneElement Element { get; set; }  // e.g., Fire, Ice, Lightning
    public int Level { get; set; }


    public Rune(string name, ERarityLevel rarity, Sprite image, int runeID, ERuneElement element = ERuneElement.Normal, int level=1)
    {
        Name = name;
        Rarity = rarity;
        Image = image;
        RuneID = runeID;
        Element = element;
        Level = level;
        itemType = EItemType.Rune;
    }

    public override string GetItemDetails()
    {
        return $"{Name} (Rarity: {Rarity}) - Element: {Element.ToString()}, Stufe: {Level}";
    }

    public override EItemType GetItemType()
    {
        return EItemType.Rune;
    }
}

public enum ERuneElement
{
    Normal,
    Fire,
    Ice,
    Lightning
}
