using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Dictionary<EMonsterType, UnitStats> monsterStats = new Dictionary<EMonsterType, UnitStats>();

    public GameData()
    {
        foreach (EMonsterType monsterType in System.Enum.GetValues(typeof(EMonsterType)))
        {
            monsterStats[monsterType] = new UnitStats();
        }
    }

    public void UpdateMonsterStats(EMonsterType monsterType, int killed, float damageDealt = 0f, float damageTaken = 0f)
    {
        if (monsterStats.ContainsKey(monsterType))
        {
            monsterStats[monsterType].enemiesKilled += killed;

            //TODO: Implement a logic to track dmg dealt per unit
            monsterStats[monsterType].totalDamageDealt += damageDealt;
            //TODO: Implement a logic to track dmg taken per unit
            monsterStats[monsterType].totalDamageTaken += damageTaken;
        }
    }

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