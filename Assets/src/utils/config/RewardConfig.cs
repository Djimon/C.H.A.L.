using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klassen für das Mapping der JSON-Datei
[System.Serializable]
public class RewardConfig
{
    public BaseRewards baseRewards;
    public Dictionary<string, Dictionary<string, RewardModifiers>> rewards;
}

[System.Serializable]
public class BaseRewards
{
    public int Gold;
    public int XP;
    public int Crystals;
}



[System.Serializable]
public class RewardModifiers
{
    public Dictionary<string, string> modifiers;

    public int Gold;
    public int XP;
    public int Crystals;
}




