using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{

    //"echte"" Event
    public static event Action<Unit, EMonsterType, EUnitSize> OnUnitKilled;

    // Beispiel-Events für verschiedene Aktionen
    public static event Action OnBossKilled;
    public static event Action<float> OnDistanceTraveled;
    public static event Action<int> OnLootCollected;
    public static event Action OnLevelUp;
    public static event Action OnGamePlayed;
    public static event Action OnSpellCast;

    // Methoden, um "echte" Events auszulösen
    public static void TriggerUnitKilled(Unit Killerunit,EMonsterType victimType, EUnitSize victimSize) => OnUnitKilled?.Invoke(Killerunit,victimType,victimSize);


    //Beispiele
    public static void TriggerBossKilled() => OnBossKilled?.Invoke();
    public static void TriggerDistanceTraveled(float distance) => OnDistanceTraveled?.Invoke(distance);
    public static void TriggerLootCollected(int amount) => OnLootCollected?.Invoke(amount);
    public static void TriggerLevelUp() => OnLevelUp?.Invoke();
    public static void TriggerGamePlayed() => OnGamePlayed?.Invoke();
    public static void TriggerSpellCast() => OnSpellCast?.Invoke();
}
