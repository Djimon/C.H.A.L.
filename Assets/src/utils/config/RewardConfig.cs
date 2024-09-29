using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klassen für das Mapping der JSON-Datei
[System.Serializable]
public class RewardConfig
{
    public BaseRewards baseRewards;
    public List<RewardModifierEntry> rewardModifiers = new List<RewardModifierEntry>();
}

[System.Serializable]
public class BaseRewards
{
    public int Gold;
    public int XP;
    public int Crystals;
}

[System.Serializable]
public class RewardModifierEntry
{
    public string monsterType; // z.B. "Chitinoid"
    public List<RewardModifiers> modifiers = new List<RewardModifiers>();
}


[System.Serializable]
public class RewardModifiers
{
    public string unitSize; // z.B. "small", "medium", etc.
    public float Gold;
    public float XP;
    public float Crystals;
}

[System.Serializable]
public class RawRewardModifiers
{
    public string monsterType;
    public List<RawRewardSizes> monsterSize;
}

[System.Serializable]
public class RawRewardSizes
{
    public string size;
    public BaseRewards rewardModifiers ;
}



[System.Serializable]
public class RawRewardConfig
{
    public BaseRewards baseRewards;
    public List<RawRewardModifiers> RewardMultilpier; // Verwenden von Dictionary nur intern
}







