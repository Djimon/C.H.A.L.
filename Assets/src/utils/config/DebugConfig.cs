using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebugConfig
{
    public DebugManager.EDebugLevel DebugLevel = DebugManager.EDebugLevel.Production;
    public bool ProductiveMode = false;

    public HashSet<string> ActiveTags = new HashSet<string>();
    public HashSet<string> ExcludedTags = new HashSet<string>();

    public void ApplyConfig(string property, string value)
    {
        switch (property)
        {
            case "DebugLevel":
                if (int.TryParse(value, out int level))
                {
                    DebugLevel = (DebugManager.EDebugLevel)level;
                }
                break;

            case "ProductiveMode":
                if (bool.TryParse(value, out bool productiveMode))
                {
                    ProductiveMode = productiveMode;
                }
                break;

            case "ActiveTags":
                value = value.Trim('[', ']');
                string[] tags = value.Split(',');
                foreach (string tag in tags)
                {
                    ActiveTags.Add(tag.Trim());
                }
                break;
            case "ExcludedTags":
                value = value.Trim('[', ']');
                string[] excludedtags = value.Split(',');
                foreach (string tag in excludedtags)
                {
                    //ExcludedTags.Add(tag.Trim());
                }
                break;
            default:
                DebugManager.Warning($"unkown property {property},", 1);
                break;
        }
    }

    public void ApplySettings()
    {
        DebugManager.SetDebugLevel(DebugLevel);
        DebugManager.SetProductiveMode(ProductiveMode);

        // Alle Tags deaktivieren und nur die in der Konfiguration angegebenen aktivieren
        foreach (string tag in ActiveTags)
        {
            DebugManager.SetTagActive(tag, true);
        }

        foreach (string tag in ExcludedTags)
        {
            DebugManager.SetTagActive(tag, false);
        }

        DebugManager.Log($"Debug-Einstellungen angewendet: Level={DebugLevel}, ProductiveMode={ProductiveMode}, Tags={string.Join(", ", ActiveTags)}");
    }
}
