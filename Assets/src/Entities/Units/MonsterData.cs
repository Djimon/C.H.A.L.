using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData 
{
    public int monsterID;
    public string monsterName;
    public EMonsterType monsterType;
    public EUnitSize monsterUnitSize;

    public bool hasHorn;
    public bool hasStinger;
    public bool hasPincers;
    public bool hasPoison;

    public string lootTableName;

    public void Initialize()
    {
        //TODO: Abh�ngig der Boolwerte und des MonsterType den benutzten Loottable w�hlen
        lootTableName = "default_drop";
    }

}
