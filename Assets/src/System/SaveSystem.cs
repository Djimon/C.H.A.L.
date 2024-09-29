using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gamedata.dat");
    }

    // Speichert die GameData als Binärdatei
    public void SaveGameData(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            formatter.Serialize(stream, data);
            DebugManager.Log("Game data saved.");
        }
    }

    // Lädt die GameData aus der Binärdatei
    public GameData LoadGameData()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                GameData data = formatter.Deserialize(stream) as GameData;
                DebugManager.Log("Game data loaded.");
                return data;
            }
        }
        else
        {
            DebugManager.Warning("Save file not found, returning new GameData.",1,"Game");
            return new GameData(); // Standardwerte zurückgeben, falls Datei nicht existiert
        }
    }

    // Löscht die gespeicherte Datei
    public void DeleteGameData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            DebugManager.Log("Game data deleted.");
        }
        else
        {
            DebugManager.Warning("No save file to delete.",1,"");
        }
    }
}
