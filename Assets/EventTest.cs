using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EvenTest : MonoBehaviour
{
    public LootTable LootTable;


    private void Update()
    {
        // Test-Events auslösen
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventManager.TriggerEnemyKilled(); // Normaler Gegner getötet
            DebugManager.Log("Pressed K");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            EventManager.TriggerBossKilled(); // Boss getötet
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventManager.TriggerDistanceTraveled(10f); // 10 Meter gelaufen
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventManager.TriggerLootCollected(1); // 1 Loot eingesammelt
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            EventManager.TriggerLevelUp(); // Level Up
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            EventManager.TriggerGamePlayed(); // Spiel gespielt
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            EventManager.TriggerSpellCast(); // Zauber gewirkt
        }
    }

}

