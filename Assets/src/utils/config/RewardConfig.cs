using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klassen f�r das Mapping der JSON-Datei
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
    public List<RawRewardSizes> monsterSizes;
}

[System.Serializable]
public class RawRewardSizes
{
    public string size;
    public RawRewards rewardModifiers; // integer, interpreted as %-values
}

[System.Serializable]
public class RawRewards
{
    public int Gold; // 100% = 1
    public int XP; // 50% = 0.5
    public int Crystals; // 200% = 2
}

[System.Serializable]
public class RawRewardConfig
{
    public BaseRewards baseRewards;
    public List<RawRewardModifiers> RewardMultilpier; // Verwenden von Dictionary nur intern
}







