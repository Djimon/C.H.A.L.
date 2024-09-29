using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public RewardConfig rewardConfig;

    public int CalculateReward(EMonsterType monsterType, EUnitSize monsterSize, string rewardType)
    {
        // Basisbelohnung für diesen Reward-Typ (Gold, XP, Crystals)
        float baseReward = GetBaseReward(rewardType);
        string monsterTypeKey = monsterType.ToString();
        string monsterSizeKey = monsterSize.ToString();

        if (rewardConfig.rewards.ContainsKey(monsterTypeKey) && 
            rewardConfig.rewards[monsterTypeKey].ContainsKey(monsterSizeKey))
        {
            RewardModifiers modifiers = rewardConfig.rewards[monsterTypeKey][monsterSizeKey];

            // Wende den entsprechenden Modifikator an, falls er vorhanden ist
            float multiplier = GetMultiplier(modifiers, rewardType);
            return (int) MathF.Round(baseReward * multiplier,0);
        }

        // Wenn keine spezifischen Belohnungen gefunden werden, wird 0 zurückgegeben
        return 0;
    }

    private float GetBaseReward(string rewardType)
    {
        switch (rewardType)
        {
            case "Gold":
                return rewardConfig.baseRewards.Gold;
            case "XP":
                return rewardConfig.baseRewards.XP;
            case "Crystals":
                return rewardConfig.baseRewards.Crystals;
            default:
                return 0;
        }
    }

    private float GetMultiplier(RewardModifiers modifiers, string rewardType)
    {
        switch (rewardType)
        {
            case "Gold":
                return modifiers.Gold;
            case "XP":
                return modifiers.XP;
            case "Crystals":
                return modifiers.Crystals;
            default:
                return 0;
        }
    }

    internal int RewardXP(EMonsterType type, EUnitSize size)
    {
        return CalculateReward(type, size, "XP");
    }

    internal int RewardGold(EMonsterType type, EUnitSize size)
    {
        return CalculateReward(type, size, "Gold");
    }

    internal int RewardCrystals(EMonsterType type, EUnitSize size)
    {
        return CalculateReward(type, size, "Crystals");
    }
}
