using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveSystem))]
[RequireComponent(typeof(ConfigurationLoader))]
[RequireComponent(typeof(CentralBank))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(LootSystem))]
public class GameManager : MonoBehaviour
{
    private GameData gameData;
    private SaveSystem saveSystem;
    private CentralBank centralBank;
    private InventoryManager inventoryManager;
    private LootSystem lootSystem;
    private ConfigurationLoader configurationLoader;


    private void Awake()
    {
        centralBank = GetComponent<CentralBank>();
        saveSystem = GetComponent<SaveSystem>();
        inventoryManager = GetComponent<InventoryManager>();
        lootSystem = GetComponent<LootSystem>();
        configurationLoader = GetComponent<ConfigurationLoader>();
        gameData = saveSystem.LoadGameData();
    }

    public void Start()
    {
        //TODO: For each registered player
        centralBank.CreatePlayerCurrencies(1);
        centralBank.AddCurrencyForPLayer(1, "XP", ECurrencyType.reward);
        centralBank.AddCurrencyForPLayer(1, "Gold", ECurrencyType.money, 100);
        centralBank.AddCurrencyForPLayer(1, "Seelenkristalle", ECurrencyType.special, 0, 1000);

        inventoryManager.CreatePlayerInventory(1);

        gameData.gamestartet = true;
        gameData.PlayerID = 1;

    }

    private void OnEnable()
    {
        //Fertige Events abonieren
        EventManager.OnUnitKilled += UpdateUnitKilled;

        //Beispiel-Events abonnieren
        EventManager.OnBossKilled += UpdateBossKilled;
        EventManager.OnDistanceTraveled += UpdateDistanceTraveled;
        EventManager.OnLootCollected += UpdateLootCollected;
        EventManager.OnLevelUp += UpdateLevelUp;
        EventManager.OnGamePlayed += UpdateGamePlayed;
        EventManager.OnSpellCast += UpdateSpellCast;
    }

    private void OnDisable()
    {
        //fertige Ebents abbestellen
        EventManager.OnUnitKilled -= UpdateUnitKilled;

        //Beispiel-Events abbestellen
        EventManager.OnBossKilled -= UpdateBossKilled;
        EventManager.OnDistanceTraveled -= UpdateDistanceTraveled;
        EventManager.OnLootCollected -= UpdateLootCollected;
        EventManager.OnLevelUp -= UpdateLevelUp;
        EventManager.OnGamePlayed -= UpdateGamePlayed;
        EventManager.OnSpellCast -= UpdateSpellCast;
    }

    private void UpdateUnitKilled(Unit unit, EMonsterType type, EUnitSize size)
    {
        lootSystem.AddLootFromMobKill(unit.TeamNumber, type);
        centralBank.GivePlayerCurrency(1, "XP",configurationLoader.CalculateReward(type,size,"XP"));
        centralBank.GivePlayerCurrency(1, "Gold", configurationLoader.CalculateReward(type, size, "Gold"));
        centralBank.GivePlayerCurrency(1, "Crystals", configurationLoader.CalculateReward(type, size, "Crystals"));
        
        gameData.totalEnemiesKilled++;
        gameData.UpdateMonsterStats(type, 1);

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

