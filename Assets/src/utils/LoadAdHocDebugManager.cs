using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadAdhocDebugManager : MonoBehaviour
{
    public DebugConfig debugConfig;

    private void Awake()
    {
        LoadDebugConfig();
    }

    private void LoadDebugConfig()
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
