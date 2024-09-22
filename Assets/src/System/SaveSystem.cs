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
            Debug.Log("Game data saved.");
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
                Debug.Log("Game data loaded.");
                return data;
            }
        }
        else
        {
            Debug.LogWarning("Save file not found, returning new GameData.");
            return new GameData(); // Standardwerte zurückgeben, falls Datei nicht existiert
        }
    }

    // Löscht die gespeicherte Datei
    public void DeleteGameData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Game data deleted.");
        }
        else
        {
            Debug.LogWarning("No save file to delete.");
        }
    }
}
