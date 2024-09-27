using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LootSerializer
{
    public void SaveLootTable(RefinedLootTable lootTable, string filePath)
    {
        if (File.Exists(filePath))
        {
            Debug.LogWarning($"LootTable not saved. File already exists at {filePath}");
            return; // Wenn die Datei existiert, verlasse die Methode ohne zu speichern
        }

        int value = (int)lootTable.type;
        var enumname = (ELootTableType)value;
        string enumnamestring = enumname.ToString();
        Debug.Log($"{value} - {enumname} - {enumnamestring}");
        //droppt das property name, das nur für den filePath benötigt wird
        ProcessedLootTable rawLootTable = new ProcessedLootTable()
        {
            type = enumnamestring,
            pools = lootTable.pools,
        };

        // Konvertiere die rohe Struktur in JSON
        string json = JsonUtility.ToJson(rawLootTable, true); // 'true' für lesbaren JSON

        // Speichere die JSON-Datei
        File.WriteAllText(filePath, json);
        Debug.Log($"LootTable saved to {filePath}");
    }

   
}

[System.Serializable]
public class RefinedLootTable
{
    public string Name;
    public ELootTableType type;  // Keep as string to process later
    public List<RawPool> pools;
}

public class ProcessedLootTable
{
    public string type;
    public List<RawPool> pools;
}
