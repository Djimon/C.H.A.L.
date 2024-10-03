using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ELanguages
{
    English,
    German
}

public static class LocalisationManager
{
    public static ELanguages currentLanguage = ELanguages.English;

    private static Dictionary<string, string> currentTranslations = new Dictionary<string, string>();

    public static void Initiliaize()
    {
        LoadTranslations();
    }

    public static void ChangeLanguage(ELanguages newLang)
    {
        currentLanguage = newLang;
        LoadTranslations();
        DebugManager.Log($"Changed language to {currentLanguage.ToString()}.",1,"Info",Color.green);
    }

    private static void LoadTranslations()
    {
        string fileSuffix = GetFileSuffixForLanguage(currentLanguage);
        string filePath = Path.Combine(Application.dataPath, $"config/lang/localisation_{fileSuffix}.json");

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            TranslationEntry[] translatoins = JsonUtility.FromJson<RawTranslation>(jsonContent).Entries;

            currentTranslations = new Dictionary<string, string>();
            foreach(var entry in translatoins)
            {
                currentTranslations[entry.key] = entry.translation;
            }
        }
        else
        {
            Debug.LogWarning("Translation file not found: " + filePath);
        }
    }

    private static string GetFileSuffixForLanguage(ELanguages currentLanguage)
    {
        return currentLanguage switch
        {
            ELanguages.English => "en-US",
            ELanguages.German => "de-DE",
            _ => "en-US"
            
        };
    }

    public static string GetTranslation(string originalName)
    {
        if (currentTranslations.ContainsKey(originalName))
        {
            return currentTranslations[originalName];
        }

        // If no translation is found, return the original name
        return originalName;
    }
}

[System.Serializable]
public class RawTranslation
{
    public TranslationEntry[] Entries;
}

[System.Serializable]
public class TranslationEntry
{
    public string key;
    public string translation;
}
