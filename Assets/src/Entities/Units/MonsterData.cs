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
        //TODO: Abhängig der Boolwerte und des MonsterType den benutzten Loottable wählen
        lootTableName = "default_drop";
    }

}
