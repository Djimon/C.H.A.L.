using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrencies
{
    private Dictionary<string, Currency> currencies;

    public PlayerCurrencies()
    {
        currencies = new Dictionary<string, Currency>();
    }

    // Eine neue Währung für den Spieler hinzufügen
    public void AddCurrency(string currencyName, ECurrencyType type, int initAmount =0, int maxAmount =0)
    {
        if (!currencies.ContainsKey(currencyName))
        {
            currencies[currencyName] = new Currency(currencyName, type, initAmount, maxAmount);
        }
    }

    public void AddCurrencyAmount(string currencyName, int amount)
    {
        int rest = 0;
        if (currencies.ContainsKey(currencyName))
        {
            rest = currencies[currencyName].Add(amount);
            DebugManager.Log($"{amount} {currencyName} was given to player.",2,"Info",Color.green);
        }

        if (rest > 0)
        {
            //TODO: Do somthing with the rest, e.g. Lokc in Tresor
            DebugManager.Log($"max Capacity of {currencyName} reached. rest of {rest} is gone!",2,"Info");
        }
    }

    public bool SpendCurrency(string currencyName, int amount)
    {
        if (currencies.ContainsKey(currencyName))
        {
            return currencies[currencyName].Subtract(amount);
        }
        return false;
    }

    public int GetCurrencyAmount(string currencyName)
    {
        if (currencies.ContainsKey(currencyName))
        {
            return currencies[currencyName].Balance;
        }
        return 0;
    }
}
