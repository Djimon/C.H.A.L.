using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class UnitStats
{
    public int enemiesKilled;
    public float totalDamageDealt;
    public float totalDamageTaken;

    public UnitStats(int killed = 0, float damageDealt = 0, float damageTaken = 0)
    {
        enemiesKilled = killed;
        totalDamageDealt = damageDealt;
        totalDamageTaken = damageTaken;
    }

    public void PrintStats(EMonsterType monsterType)
    {
        DebugManager.Log($"{monsterType} - Enemies Killed: {enemiesKilled}, Damage Dealt: {totalDamageDealt}, Damage Taken: {totalDamageTaken}");
    }
}
