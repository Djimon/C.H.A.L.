using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ShowCurrencies : MonoBehaviour
{
    public GameObject gameManger;
    public int playerID = 1;

    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textCrystals;
    public TextMeshProUGUI textXP;

    private CentralBank bank;

    // Start is called before the first frame update
    void Start()
    {
        bank = gameManger.GetComponent<CentralBank>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateGold();
        UpdateCrystals();
        UpdateXP();
    }

    public void UpdateGold()
    {
        int gold = bank.GetPlayerCurrencyBallance(playerID, "Gold");
        textGold.text = FormatHelper.CleanNumber(gold);
       
    }

    public void UpdateCrystals()
    {
        int crystals = bank.GetPlayerCurrencyBallance(playerID, "Crystals");
        textCrystals.text = FormatHelper.CleanNumber(crystals);
    }

    public void UpdateXP()
    {
        int xp = bank.GetPlayerCurrencyBallance(playerID, "XP");
        textXP.text = FormatHelper.CleanNumber(xp);
    }
}
