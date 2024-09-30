using UnityEngine;

[CreateAssetMenu(fileName = "NewRune", menuName = "Items/Rune")]
public class ScriptableRune : ScriptableItemBase
{
    public int runeID;
    public ERuneElement Element;  // e.g., Fire, Ice, Lightning
    public int Level;

    public override Item CreateInstance()
    {
        return new Rune(name, rarity,image,runeID, Element, Level);
    }

    // Override to provide PowerUp-specific details
    public override string GetItemDetails()
    {
        return $"{name} (Rarity: {rarity}) - Element: {Element.ToString()}, Stufe: {Level}";
    }
}
