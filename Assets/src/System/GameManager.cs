using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SaveSystem))]
[RequireComponent(typeof(ConfigurationLoader))]
[RequireComponent(typeof(CentralBank))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(LootSystem))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GamePhase currentPhase;

    public List<GameObject> managingObjects;
    

    private GameData gameData;
    private SaveSystem saveSystem;
    private CentralBank centralBank;
    private InventoryManager inventoryManager;
    private LootSystem lootSystem;
    private ConfigurationLoader configurationLoader;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Erlaubt es, dass der GameManager zwischen Szenen bestehen bleibt
        }
        else
        {
            Destroy(gameObject);
        }

        centralBank = GetComponent<CentralBank>();
        saveSystem = GetComponent<SaveSystem>();
        inventoryManager = GetComponent<InventoryManager>();
        lootSystem = GetComponent<LootSystem>();
        configurationLoader = GetComponent<ConfigurationLoader>();
        gameData = saveSystem.LoadGameData();

        LocalisationManager.Initiliaize();
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

    public void Update()
    {
        if (GameManager.instance.currentPhase == GamePhase.ManagingPhase)
        {
            ToggleManagingObjects(true);
        }
        else if (GameManager.instance.currentPhase == GamePhase.BattlePhase)
        {
            ToggleManagingObjects(false);
        }
    }

    private void ToggleManagingObjects(bool active)
    {
        foreach (GameObject go in managingObjects)
        {
            //Only disable Menus and Klick-Events, Models like the forge and the laboratory should still be visible
            go.SetActive(active);
        }
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

    private void UpdateUnitKilled(Unit unit, MonsterData victim)
    {
        
        inventoryManager.AddItemsForPlayer(unit.TeamNumber, lootSystem.GetLootFromMobKill(unit.TeamNumber, victim));
        centralBank.GivePlayerCurrency(1, "XP",configurationLoader.CalculateReward(victim.monsterType,victim.monsterUnitSize,"XP"));
        centralBank.GivePlayerCurrency(1, "Gold", configurationLoader.CalculateReward(victim.monsterType, victim.monsterUnitSize, "Gold"));
        centralBank.GivePlayerCurrency(1, "Crystals", configurationLoader.CalculateReward(victim.monsterType, victim.monsterUnitSize, "Crystals"));
        
        gameData.totalEnemiesKilled++;
        gameData.UpdateMonsterStats(victim.monsterType, 1);

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

    public void ChangePhase(GamePhase newPhase)
    {
        currentPhase = newPhase;

        switch (newPhase)
        {
            case GamePhase.StartScreen:
                SceneManager.LoadScene("StartScreen");  // Wechsel zur Startbildschirmszene
                break;

            case GamePhase.CharacterCreation:
                SceneManager.LoadScene("CharacterCreation");  // Wechsel zur Charaktererstellungsszene
                break;

            case GamePhase.ManagingPhase:
                // Wenn du den Wechsel zwischen ManagingPhase und Kampfphase in derselben Szene machst
                StartManagingPhase();
                break;

            case GamePhase.BattlePhase:
                StartBattlePhase();
                break;
            case GamePhase.GameOver:
                SceneManager.LoadScene("GameOver");  // Wechsel zur Charaktererstellungsszene
                break;
        }
    }

    private void StartManagingPhase()
    {
        Debug.Log("Managing Phase gestartet");
        // Starte UI f�r das Ausw�hlen von Einheiten, Ressourcenmanagment etc.
        // Hier kannst du z.B. das UI f�r diese Phase aktivieren
    }

    private void StartBattlePhase()
    {
        Debug.Log("Kampf Phase gestartet");
        // Starte den Kampf, spawne Einheiten, aktiviere die Kampf-Logik etc.
    }

    public void OnBattleEnd(bool playerWon)
    {
        if (playerWon)
        {
            // Kampfphase beendet, Spieler hat gewonnen, gehe zur�ck zur Managingphase
            ChangePhase(GamePhase.ManagingPhase);
        }
        else
        {
            // Spieler hat verloren -> Game Over oder Neustart
            Debug.Log("Spieler verloren, Game Over");
            ChangePhase(GamePhase.StartScreen);
        }
    }


}

public enum GamePhase
{
    StartScreen,
    CharacterCreation,
    ManagingPhase,
    BattlePhase,
    LootingPhase,
    GameOver
}

