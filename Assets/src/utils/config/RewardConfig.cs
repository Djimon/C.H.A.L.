using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klassen für das Mapping der JSON-Datei
[System.Serializable]
public class RewardConfig
{
    public BaseRewards baseRewards;
    public Rewards rewards;
}

[System.Serializable]
public class BaseRewards
{
    public int Gold;
    public int XP;
    public int Crystals;
}

[System.Serializable]
public class RewardSize
{
    public float Gold;
    public float XP;
    public float Crystals;
}

[System.Serializable]
public class Rewards
{
    public Dictionary<string, Dictionary<string, RewardSize>> rewards;
}


