using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ScriptableTemplate : ScriptableItemBase
{

    // Override to provide PowerUp-specific details
    public override string GetItemDetails()
    {
        return $"{itemName} (Rarity: {rarity}) is not implemented yet.";
    }

}

