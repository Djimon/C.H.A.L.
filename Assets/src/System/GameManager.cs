using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveSystem))]
[RequireComponent(typeof(ConfigurationLoader))]
[RequireComponent(typeof(CentralBank))]
public class GameManager : MonoBehaviour
{
    private GameData gameData;
    private SaveSystem saveSystem;
    private CentralBank centralBank;

    private void Awake()
    {
        centralBank = GetComponent<CentralBank>();
        saveSystem = GetComponent<SaveSystem>();
        gameData = saveSystem.LoadGameData();
    }

    public void Start()
    {
        //TODO: For each registered player
        centralBank.CreatePlayerCurrencies(1);
        centralBank.AddCurrencyForPLayer(1, "Gold", ECurrencyType.money, 100);
        centralBank.AddCurrencyForPLayer(1, "Seelenkristalle", ECurrencyType.special, 0, 1000);

        gameData.gamestartet = true;
        gameData.PlayerID = 1;
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
        DebugManager.Log("Enemy killed");
    }

    private void UpdateBossKilled()
    {
        gameData.bossEnemiesKilled++;
        DebugManager.Log("Boss killed");
    }

    private void UpdateDistanceTraveled(float distance)
    {
        gameData.totalDistanceTraveled += distance;
        DebugManager.Log($"Distance traveled: {distance}");
    }

    private void UpdateLootCollected(int amount)
    {
        gameData.totalLootCollected += amount;
        DebugManager.Log($"Loot collected: {amount}");
    }

    private void UpdateLevelUp()
    {
        gameData.playerLevel++;
        DebugManager.Log("Level up!");
    }

    private void UpdateGamePlayed()
    {
        gameData.totalGamesPlayed++;
        DebugManager.Log("Game played");
    }

    private void UpdateSpellCast()
    {
        gameData.spellsCast++;
        DebugManager.Log("Spell cast");
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
    public bool gamestartet;
    public int PlayerID;
    public int totalEnemiesKilled;
    public int bossEnemiesKilled;
    public float totalDistanceTraveled;
    public int totalLootCollected;
    public int playerLevel;
    public int totalGamesPlayed;
    public int spellsCast;

    public void PrintData()
    {
        DebugManager.Log($"Total Enemies Killed: {totalEnemiesKilled}");
        DebugManager.Log($"Boss Enemies Killed: {bossEnemiesKilled}");
        DebugManager.Log($"Total Distance Traveled: {totalDistanceTraveled}");
        DebugManager.Log($"Total Loot Collected: {totalLootCollected}");
        DebugManager.Log($"Player Level: {playerLevel}");
        DebugManager.Log($"Total Games Played: {totalGamesPlayed}");
        DebugManager.Log($"Spells Cast: {spellsCast}");
    }
}
