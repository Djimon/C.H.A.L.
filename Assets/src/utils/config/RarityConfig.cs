using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RarityConfigEntry
{
    public string rarity;
    public float priceMultiplier;
    public float dnaMultiplier;
}

[System.Serializable]
public class RarityConfig
{
    public List<RarityConfigEntry> Rarities;
}
