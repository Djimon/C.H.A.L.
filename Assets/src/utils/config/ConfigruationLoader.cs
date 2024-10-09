using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static DebugManager;
using static UnityEditor.Progress;

public class ConfigurationLoader : MonoBehaviour
{
    public static ConfigurationLoader Instance { get; private set; }

    private GameManager gameManager;

    public RarityConfig rarityConfig;
    public DebugConfig debugConfig;
    public RewardConfig rewardConfig;
    public RawRewardConfig tempRewardConfig;

    private void Awake()
    {
        // Singleton Pattern, damit diese Klasse überall zugreifbar ist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadRarityConfig();
            LoadDebugConfig();
            LoadRewardsConfig();
        }

        gameManager = GetComponent<GameManager>();

    }

    private void Start()
    {

    }

    public void LoadRarityConfig()
    {
        //string path = Path.Combine(Application.streamingAssetsPath, "rarityConfig.json");
        string path = Path.Combine(Application.dataPath, "config/rarityConfig.json");


        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            rarityConfig = JsonUtility.FromJson<RarityConfig>(jsonContent);
        }
        else
        {
            DebugManager.Error("Konfigurationsdatei für Seltenheit nicht gefunden.");
        }
    }

    public RarityConfigEntry GetConfigForRarity(string rarity)
    {
        return rarityConfig.Rarities.Find(entry => entry.rarity == rarity);
    }

    public void LoadDebugConfig()
    {
        string path = Path.Combine(Application.dataPath, "config/debug.config");

        if (File.Exists(path))
        {
            string[] configLines = File.ReadAllLines(path);
            debugConfig = new DebugConfig();
            Debug.Log("Initialize DebugManager");

            foreach (string line in configLines)
            {
                string cleanLine = line.Split('#')[0].Trim();
                if (string.IsNullOrWhiteSpace(cleanLine)) continue;

                string[] parts = cleanLine.Split('=');
                if (parts.Length != 2) continue;

                string property = parts[0].Trim();
                string value = parts[1].Trim();

                debugConfig.ApplyConfig(property, value);
            }

            // Debug-Konfigurationswerte anwenden
            debugConfig.ApplySettings();
        }
        else
        {
            DebugManager.Error("Debug-Konfigurationsdatei nicht gefunden.");
        }
    }

    private void LoadRewardsConfig()
    {
        string path = Path.Combine(Application.dataPath, "config/RewardConfig.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            tempRewardConfig = JsonUtility.FromJson<RawRewardConfig>(json);
            rewardConfig = new RewardConfig
            {
                baseRewards = tempRewardConfig.baseRewards,
                rewardModifiers = new List<RewardModifierEntry>()
            };

            foreach(RawRewardModifiers rawModifiers in tempRewardConfig.RewardMultilpier)
            {
                RewardModifierEntry entry = new RewardModifierEntry
                {
                    monsterType = rawModifiers.monsterType,
                    modifiers = new List<RewardModifiers>()
                };

                rewardConfig.rewardModifiers.Add(entry);

                foreach (RawRewardSizes rawSize in rawModifiers.monsterSizes)
                {
                    
                    RewardModifiers modifier = new RewardModifiers
                    {
                        unitSize = rawSize.size
                    };

                    float _XP = rawSize.rewardModifiers.XP;
                    float _Gold = rawSize.rewardModifiers.Gold;
                    float _Crystals = rawSize.rewardModifiers.Crystals;

                    //interpret values as %
                    modifier.XP =  (1+(_XP /100));
                    modifier.Gold =  (1+(_Gold /100));
                    modifier.Crystals =  (1 + (_Crystals / 100));

                    entry.modifiers.Add(modifier);

                }

            }


            Debug.Log("Rewards config loaded successfully!");
        }
        else
        {
            Debug.LogError("Rewards config file not found: " + path);
        }
    }


    // Speichere die aktuelle RewardData in eine JSON-Datei
    public void SaveRewardsToJson(RewardConfig config)
    {
        string path = Path.Combine(Application.dataPath, "config/RewardConfig.json");
        string json = JsonUtility.ToJson(config, true); // `true` formatiert die JSON schön
        File.WriteAllText(path, json);
        Debug.Log("Rewards config saved successfully to " + path);
    }

    public int CalculateReward(EMonsterType monsterType, EUnitSize monsterSize, string rewardType)
    {
        // Basisbelohnung für diesen Reward-Typ (Gold, XP, Crystals)
        float baseReward = GetBaseReward(rewardType);
        string monsterTypeKey = monsterType.ToString();
        string monsterSizeKey = monsterSize.ToString();

        var modifier = rewardConfig.rewardModifiers
        .FirstOrDefault(entry => entry.monsterType == monsterTypeKey)?
        .modifiers.FirstOrDefault(mod => mod.unitSize == monsterSizeKey);

        if (modifier != null)
        {
            // Wende den entsprechenden Modifikator an, falls er vorhanden ist
            float multiplier = GetMultiplier(modifier, rewardType);
            int reward = (int)MathF.Round(baseReward * multiplier, 0);
            DebugManager.Log($"reward {rewardType} for {monsterSize.ToString()} {monsterType.ToString()} is {reward}.", 2, "System");
            return reward;
        }

        DebugManager.Log($"reward {rewardType} for {monsterSize.ToString()} {monsterType.ToString()} is {baseReward}.", 2, "System");

        return (int)baseReward;
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


    private void OnDisable()
    {
        WriteUpdatedConfig();
    }

    private void WriteUpdatedConfig()
    {
        string path = Path.Combine(Application.dataPath, "config/debug.config");

        bool tagExlcudedfound = false;

        if (File.Exists(path))
        {
            List<string> lines = new List<string>(File.ReadAllLines(path));

            // Finde die Zeile für ExcludedTags und aktualisiere sie
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("ExcludedTags"))
                {
                    tagExlcudedfound = true;
                    lines[i] = $"ExcludedTags = [{string.Join(", ", DebugManager.ExcludedTags)}]";
                    break;
                }
            }

            if(!tagExlcudedfound)
            {
                lines.Add($"ExcludedTags = [{string.Join(", ", DebugManager.ExcludedTags)}]");
            }

            // Schreibe die aktualisierte Konfiguration zurück in die Datei
            File.WriteAllLines(path, lines);
            DebugManager.Log("ExcludedTags in die Konfiguration geschrieben: " + string.Join(", ", string.Join(", ", DebugManager.ExcludedTags)));
        }
        else
        {
            List<string> Lines = new List<string>();
            Lines.Add("# Global Config for Deubg-System");
            Lines.Add("");
            Lines.Add("# ProductiveMode - when set to true, only Production level logs (Level 1) are shown.");
            Lines.Add("ProductiveMode = false");
            Lines.Add("");
            Lines.Add("# Set the global debug level:");
            Lines.Add("# 1 = Production, 2 = Test, 3 = Debug");
            Lines.Add("DebugLevel = 3");
            Lines.Add("");
            Lines.Add("# Tags to include in debug messages. Specify as a comma-separated list inside square brackets.");
            Lines.Add($"ActiveTags = [{string.Join(", ", DebugManager.ActiveTags)}]");
            Lines.Add("# just for Info, all Tags that are in the game and are not included");
            Lines.Add($"ExcludedTags = [{string.Join(", ", DebugManager.ExcludedTags)}]");

            File.WriteAllLines(path, Lines);
        }
    }
}




