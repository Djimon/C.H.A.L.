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
        
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            DebugManager.Log("Game data saved.");
        }
    }

    // Lädt die GameData aus der Binärdatei
    public GameData LoadGameData()
    {
        if (File.Exists(filePath))
        {
            
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                if (stream.Length == 0)
                {
                    DebugManager.Warning("Save file is empty, returning new GameData.", 1, "Game");
                    return new GameData(); // Standardwerte zurückgeben, falls Datei leer ist
                }

                BinaryFormatter formatter = new BinaryFormatter();
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
