using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    // Beispiel-Events für verschiedene Aktionen
    public static event Action OnEnemyKilled;
    public static event Action OnBossKilled;
    public static event Action<float> OnDistanceTraveled;
    public static event Action<int> OnLootCollected;
    public static event Action OnLevelUp;
    public static event Action OnGamePlayed;
    public static event Action OnSpellCast;

    // Methoden, um Events auszulösen
    public static void TriggerEnemyKilled() => OnEnemyKilled?.Invoke();
    public static void TriggerBossKilled() => OnBossKilled?.Invoke();
    public static void TriggerDistanceTraveled(float distance) => OnDistanceTraveled?.Invoke(distance);
    public static void TriggerLootCollected(int amount) => OnLootCollected?.Invoke(amount);
    public static void TriggerLevelUp() => OnLevelUp?.Invoke();
    public static void TriggerGamePlayed() => OnGamePlayed?.Invoke();
    public static void TriggerSpellCast() => OnSpellCast?.Invoke();
}
