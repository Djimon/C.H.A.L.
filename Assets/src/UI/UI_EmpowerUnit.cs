using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class UI_EmpowerUnit : MonoBehaviour
{
    public GameObject DetailsPanel;

    public GameObject[] Resources;

    //Add icon for each ressoruce and initilize with 0
    public GameObject HealthUsedRessourcesPanel;
    public GameObject PowerUsedRessourcesPanel;
    public GameObject SpeedUsedRessourcesPanel;
    public GameObject AttackSpeedUsedRessourcesPanel;

    private Unit UnitToEmpower;

    private Button btnIncreasePower;
    private Button btnIncreaseHealth;
    private Button btnIncreaseSpeed;
    private Button btnIncreaseAttackSpeed;

    public Button btnExit;

    private InventoryManager inventoryManager;

    private void Awake()
    {
        btnIncreasePower = PowerUsedRessourcesPanel.GetComponentInChildren<Button>();
        btnIncreaseHealth = HealthUsedRessourcesPanel.GetComponentInChildren<Button>();
        btnIncreaseSpeed = SpeedUsedRessourcesPanel.GetComponentInChildren<Button>();
        btnIncreaseAttackSpeed = AttackSpeedUsedRessourcesPanel.GetComponentInChildren<Button>();
    }

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        
        btnIncreasePower.onClick.AddListener(AddPower);
        btnIncreaseHealth.onClick.AddListener(AddHealth);
        btnIncreaseSpeed.onClick.AddListener(AddSpeed);
        btnIncreaseAttackSpeed.onClick.AddListener(AddAttackSpeed);

        btnExit.onClick.AddListener(Close);

        UpdateResourceAmounts();
        InitilizeGrid();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void InitilizeGrid()
    {
        //calculate needed cost cor each grid and display it

        UpdateGrid(PowerUsedRessourcesPanel);
        UpdateGrid(HealthUsedRessourcesPanel);
        UpdateGrid(SpeedUsedRessourcesPanel);
        UpdateGrid(AttackSpeedUsedRessourcesPanel);
    }

    private void AddAttackSpeed()
    {
        //Updat stat and subtract the items

        //show next Costs
        UpdateGrid(AttackSpeedUsedRessourcesPanel);
        UpdateDetails();
    }

    private void AddSpeed()
    {
        //Updat stat and subtract the items

        //show next Costs
        UpdateGrid(SpeedUsedRessourcesPanel);
        UpdateDetails();
    }

    private void AddHealth()
    {
        //Updat stat and subtract the items
        
        //show next Costs
        UpdateGrid(HealthUsedRessourcesPanel);
        //update the Details
        UpdateDetails();
    }

    private void AddPower()
    {
        //Updat stat and subtract the items

        //show next Costs
        UpdateGrid(PowerUsedRessourcesPanel);
        UpdateDetails();
    }

    private void UpdateDetails()
    {
        DetailsPanel.GetComponent<UI_UnitDetails>().UpdateDetails(UnitToEmpower);
    }

    public void SetUnit(Unit unit)
    {
        UnitToEmpower = unit;
    }

    private void UpdateResourceAmounts()
    {
        for (int i = 0; i < Resources.Length; i++)
        {
            TextMeshProUGUI text = Resources[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text.name == "Amount")
            {
                DebugManager.Log("corect Text found");
                string itemName = GetItemvyIndex(i);
                text.text = FormatHelper.CleanNumber(inventoryManager.GetItemAmountForPlayer(i, itemName));
            }
            
        }
    }

    private string GetItemvyIndex(int index)
    {
        return index switch
        {
            0 => "ashes",
            1 => "glitter_dust",
            2 => "blood",
            _ => null
        };

    }

    private void UpdateGrid(GameObject panel)
    {
        //show how many ressources are used for next level


    }





}
