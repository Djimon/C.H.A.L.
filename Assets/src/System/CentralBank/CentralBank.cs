using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralBank : MonoBehaviour
{
    private Dictionary<int, PlayerCurrencies> PlayerCurrencyDict = new Dictionary<int, PlayerCurrencies>();

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePlayerCurrencies(int playerID)
    {
        if (!PlayerCurrencyDict.ContainsKey(playerID))
        {
            PlayerCurrencyDict[playerID] = new PlayerCurrencies();
        }
    }


    public void AddCurrencyForPLayer(int playerID, string currencyName,ECurrencyType type ,int initialAmount = 0, int maxCapacity = 0)
    {
        if(PlayerCurrencyDict.ContainsKey(playerID))
        {
            PlayerCurrencyDict[playerID].AddCurrency(currencyName,type, initialAmount, maxCapacity);
        }
    }

    public void GivePlayerCurrency(int playerID, string currencyname, int amount)
    {
        if (PlayerCurrencyDict.ContainsKey(playerID))
        {
            PlayerCurrencyDict[playerID].AddCurrencyAmount(currencyname,amount);
        }
    }

    public void TakePlayerCurrency(int playerID, string currencyname, int amount)
    {
        if (PlayerCurrencyDict.ContainsKey(playerID))
        {
            PlayerCurrencyDict[playerID].SpendCurrency(currencyname,amount);
        }
    }

    public int GetPlayerCurrencyBallance(int playerID, string currencyName)
    {
        if (PlayerCurrencyDict.ContainsKey(playerID))
        {
            return PlayerCurrencyDict[playerID].GetCurrencyAmount(currencyName);
        }

        return 0;
    }
}
