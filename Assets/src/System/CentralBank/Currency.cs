using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECurrencyType
{
    none = -1,
    money,
    reward,
    special,
    microtransaciton
}


public class Currency
{
    public string CurrencyName;
    public ECurrencyType CurrencyType;
    public int Balance;
    public Sprite icon;

    private int MaxCapacity = 0; // 0 means infinite
    private bool hasCapcity = false;

    public Currency(string currencyName, ECurrencyType currencyType, int initialAmount = 0, int maxAmount = 0)
    {
        CurrencyName = currencyName;
        CurrencyType = currencyType;
        //make sure that MaxCapacity hold during constructing
        Balance = maxAmount>0? Mathf.Min(initialAmount,maxAmount): initialAmount;
        MaxCapacity = maxAmount;
        hasCapcity = maxAmount > 0;
    }

    public int Add(int value)
    {
        if(hasCapcity && (Balance+value)>MaxCapacity)
        {
            Balance = MaxCapacity;
            return value - (MaxCapacity-Balance);
            // 100 , 80 +30
            // 100, 100, Rest: 10
            // 30 - (100-80)
        }
        else
        {
            Balance += value;
            return 0;
        }
    }

    public bool Subtract(int value)
    {
        if (Balance >= value)
        {
            Balance -= value;
            return true;
        }
        else
        {
            DebugManager.Log($"Nicht genügend {CurrencyName}.");
            return false;
        }
    }

}
