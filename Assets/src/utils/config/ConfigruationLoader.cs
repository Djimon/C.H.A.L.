using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static DebugManager;
using static UnityEditor.Progress;

public class ConfigurationLoader : MonoBehaviour
{
    public static ConfigurationLoader Instance { get; private set; }

    public RarityConfig rarityConfig;
    public DebugConfig debugConfig;

    private void Awake()
    {
        // Singleton Pattern, damit diese Klasse überall zugreifbar ist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadRarityConfig();
            LoadDebugConfig();
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
            Lines.Add($"ExcludedTags = [{string.Join(", ", DebugManager.ActiveTags)}]");
            Lines.Add("# just for Info, all Tags that are in the game and are not included");
            Lines.Add($"ExcludedTags = [{string.Join(", ", DebugManager.ExcludedTags)}]");

            File.WriteAllLines(path, Lines);
        }
    }
}




