using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebugManager
{
    public enum EDebugLevel
    {
        Production = 1,
        Test = 2,
        Dev = 3,
        Debug = 4
    }
    // Globale Einstellungen
    public static EDebugLevel CurrentDebugLevel = EDebugLevel.Production;
    public static bool ProductiveMode = false;

    // Aktivierte Tags
    public static HashSet<string> ActiveTags = new HashSet<string>();
    public static HashSet<string> ExcludedTags = new HashSet<string>();

    private static readonly Dictionary<string, Color> tagColors = new Dictionary<string, Color>
    {
        { "System", Color.yellow },
        { "PlayerInfo", Color.blue},
        { "EventSystem", Color.green}
    };

    // Debug Methode
    public static void Log(string message, int level = 3, string tag = "System", Color? customColor = null)
    {
        // Umwandlung des int Werts in den entsprechenden EDebugLevel
        EDebugLevel debugLevel = (EDebugLevel)level;
        // Wenn ProductiveMode an ist, nur Level-1-Nachrichten ausgeben
        if (ProductiveMode && debugLevel != EDebugLevel.Production) return;

        // Überprüfen, ob die Nachricht angezeigt werden soll (basierend auf Level und Tag)
        if (debugLevel <= CurrentDebugLevel && (tag == "" || ActiveTags.Contains(tag)))
        {
            Color tagColor = customColor ?? GetTagColor(tag);
            string timeStamp = GetGameTime();
            string coloredTimeAndTag = $"<color=#{ColorUtility.ToHtmlStringRGB(tagColor)}>{tag} @ {timeStamp}s</color>";
            Debug.Log($"[{coloredTimeAndTag}]: {message}");
        }
        else if(!ActiveTags.Contains(tag)) 
        {
            ExcludedTags.Add(tag);
        }
    }

    public static void Error(string message, int level = 1, string tag = "System", Color? customColor = null)
    {
        // Umwandlung des int Werts in den entsprechenden EDebugLevel
        EDebugLevel debugLevel = (EDebugLevel)level;

        if (ProductiveMode && debugLevel != EDebugLevel.Production) return;

        if (debugLevel <= CurrentDebugLevel && (tag == "" || ActiveTags.Contains(tag)))
        {
            Color tagColor = customColor ?? GetTagColor(tag);
            string timeStamp = GetGameTime();
            string coloredTimeAndTag = $"<color=#{ColorUtility.ToHtmlStringRGB(tagColor)}>{tag} @ {timeStamp}s</color>";
            Debug.LogError($"[{coloredTimeAndTag}]: {message}");
        }
        else if (!ActiveTags.Contains(tag))
        {
            ExcludedTags.Add(tag);
        }
    }

    public static void Warning(string message, int level = 2, string tag = "System", Color? customColor = null)
    {
        // Umwandlung des int Werts in den entsprechenden EDebugLevel
        EDebugLevel debugLevel = (EDebugLevel)level;

        if(ProductiveMode && debugLevel != EDebugLevel.Production) return;

        if (debugLevel <= CurrentDebugLevel && (tag == "" || ActiveTags.Contains(tag)))
        {
            Color tagColor = customColor ?? GetTagColor(tag);
            string timeStamp = GetGameTime();
            string coloredTimeAndTag = $"<color=#{ColorUtility.ToHtmlStringRGB(tagColor)}>{tag} @ {timeStamp}s</color>";
            Debug.LogWarning($"[{coloredTimeAndTag}]: {message}");
        }
        else if (!ActiveTags.Contains(tag))
        {
            ExcludedTags.Add(tag);
        }
    }

    private static string GetGameTime()
    {
        return Time.time.ToString("F3"); // Zeit in Sekunden seit Spielstart (2 Dezimalstellen)
        // Wenn du die Echtzeit verwenden möchtest, kannst du so etwas verwenden:
        // return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private static Color GetTagColor(string tag)
    {
        if (tagColors.TryGetValue(tag, out Color color))
        {
            return color;
        }
        return Color.white; // Standardfarbe, falls kein Tag gefunden wurde
    }

    // Methode zum Aktivieren oder Deaktivieren von Tags
    public static void SetTagActive(string tag, bool isActive)
    {
        if (isActive)
        {
            ActiveTags.Add(tag);
            ExcludedTags.Remove(tag);
        }
        else
        {
            ActiveTags.Remove(tag);
            ExcludedTags.Add(tag);
        }
    }

    // Setze das Debug-Level global
    public static void SetDebugLevel(EDebugLevel level)
    {
        CurrentDebugLevel = level;
    }

    // Setze den Productive Mode
    public static void SetProductiveMode(bool productive)
    {
        ProductiveMode = productive;
    }

}
