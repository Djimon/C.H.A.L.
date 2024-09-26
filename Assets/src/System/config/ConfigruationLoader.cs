using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigurationLoader : MonoBehaviour
{
    public static ConfigurationLoader Instance { get; private set; }

    public RarityConfig rarityConfig;

    private void Awake()
    {
        // Singleton Pattern, damit diese Klasse überall zugreifbar ist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRarityConfig();
        }

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
            Debug.LogError("Konfigurationsdatei für Seltenheit nicht gefunden.");
        }
    }

    public RarityConfigEntry GetConfigForRarity(string rarity)
    {
        return rarityConfig.Rarities.Find(entry => entry.rarity == rarity);
    }
}

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
