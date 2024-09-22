using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameData gameData;
    private SaveSystem saveSystem;

    private void Awake()
    {
        saveSystem = FindObjectOfType<SaveSystem>();
        gameData = saveSystem.LoadGameData();
    }

    private void OnEnable()
    {
        // Events abonnieren
        EventManager.OnEnemyKilled += UpdateEnemyKilled;
        EventManager.OnBossKilled += UpdateBossKilled;
        EventManager.OnDistanceTraveled += UpdateDistanceTraveled;
        EventManager.OnLootCollected += UpdateLootCollected;
        EventManager.OnLevelUp += UpdateLevelUp;
        EventManager.OnGamePlayed += UpdateGamePlayed;
        EventManager.OnSpellCast += UpdateSpellCast;
    }

    private void OnDisable()
    {
        // Events abbestellen
        EventManager.OnEnemyKilled -= UpdateEnemyKilled;
        EventManager.OnBossKilled -= UpdateBossKilled;
        EventManager.OnDistanceTraveled -= UpdateDistanceTraveled;
        EventManager.OnLootCollected -= UpdateLootCollected;
        EventManager.OnLevelUp -= UpdateLevelUp;
        EventManager.OnGamePlayed -= UpdateGamePlayed;
        EventManager.OnSpellCast -= UpdateSpellCast;
    }

    // Event-Handler-Methoden zur Aktualisierung der Statistiken
    private void UpdateEnemyKilled()
    {
        gameData.totalEnemiesKilled++;
        Debug.Log("Enemy killed");
    }

    private void UpdateBossKilled()
    {
        gameData.bossEnemiesKilled++;
        Debug.Log("Boss killed");
    }

    private void UpdateDistanceTraveled(float distance)
    {
        gameData.totalDistanceTraveled += distance;
        Debug.Log($"Distance traveled: {distance}");
    }

    private void UpdateLootCollected(int amount)
    {
        gameData.totalLootCollected += amount;
        Debug.Log($"Loot collected: {amount}");
    }

    private void UpdateLevelUp()
    {
        gameData.playerLevel++;
        Debug.Log("Level up!");
    }

    private void UpdateGamePlayed()
    {
        gameData.totalGamesPlayed++;
        Debug.Log("Game played");
    }

    private void UpdateSpellCast()
    {
        gameData.spellsCast++;
        Debug.Log("Spell cast");
    }

    // Speichern der Statistiken
    public void SaveGame()
    {
        saveSystem.SaveGameData(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

[Serializable]
public class GameData
{
    public int totalEnemiesKilled;
    public int bossEnemiesKilled;
    public float totalDistanceTraveled;
    public int totalLootCollected;
    public int playerLevel;
    public int totalGamesPlayed;
    public int spellsCast;

    public void PrintData()
    {
        Debug.Log($"Total Enemies Killed: {totalEnemiesKilled}");
        Debug.Log($"Boss Enemies Killed: {bossEnemiesKilled}");
        Debug.Log($"Total Distance Traveled: {totalDistanceTraveled}");
        Debug.Log($"Total Loot Collected: {totalLootCollected}");
        Debug.Log($"Player Level: {playerLevel}");
        Debug.Log($"Total Games Played: {totalGamesPlayed}");
        Debug.Log($"Spells Cast: {spellsCast}");
    }
}
