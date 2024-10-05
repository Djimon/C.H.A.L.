using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Search;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class UI_EmpowerUnit : MonoBehaviour
{
    public GameObject DetailsPanel;

    public GameObject[] Resources;

    //TODO: Prefab erstellen
    public GameObject ressEntryPrefab;

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

    private Dictionary<string, int> playerResourceAmount = new Dictionary<string, int>();

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

    }

    private void UpdateGridUI(GameObject grid, List<ResourceCost> ressCost)
    {
        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);        
        }

        foreach (ResourceCost requirement in ressCost)
        { 
            GameObject ressEntry = Instantiate(ressEntryPrefab, grid.transform);

            ressEntry.transform.Find("Icon").GetComponent<Image>().sprite = GetRessIcon(requirement.resourceName);
            ressEntry.transform.Find("reqAmount").GetComponent<TextMeshProUGUI>().text = FormatHelper.CleanNumber(requirement.cost);
        }
        
    }

    private Sprite GetRessIcon(string resourceName)
    {
        return null;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void InitilizeGrid()
    {
        //calculate needed cost cor each grid and display it
        UpdateGrid(PowerUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.powerLevel));
        UpdateGrid(HealthUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.healthLevel));
        UpdateGrid(SpeedUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.speedLevel));
        UpdateGrid(AttackSpeedUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.attackSpeedLevel));
    }

    public List<ResourceCost> CalculateResourceCost(int resourcelevel)
    {
        List<ResourceCost> requirements = new List<ResourceCost>();

        int ressOnecost = 100 * (int)Mathf.Pow(2, resourcelevel);
        requirements.Add(new ResourceCost() { resourceName = "ashes", cost = ressOnecost });

        if (resourcelevel >= 3)
        {
            int ressTwoCost = 50 * (int)Mathf.Pow(2, resourcelevel - 3);
            requirements.Add(new ResourceCost() { resourceName = "glitter_dust", cost = ressTwoCost });
        }

        if (resourcelevel >= 10)
        {
            int ressThreeCost = 25 * (int)Mathf.Pow(2, resourcelevel - 10);
            requirements.Add(new ResourceCost() { resourceName = "blood", cost = ressThreeCost });
        }

        return requirements;
    }

    private void AddPower()
    {
        int currentLevel = UnitToEmpower.powerLevel;
        List<ResourceCost> nextLevelRequirements = CalculateResourceCost(UnitToEmpower.powerLevel);

        // Überprüfen, ob der Spieler genug Ressourcen hat
        if (PlayerHasEnoughResources(nextLevelRequirements))
        {
            // Ressourcen abziehen
            SubtractResources(nextLevelRequirements);

            // Level erhöhen
            UnitToEmpower.IncreaseAttackPower();

            // UI aktualisieren
            UpdateGrid(PowerUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.powerLevel));
            //update the Details
            UpdateDetails();
        }
        else
        {
            Debug.Log("Nicht genügend Ressourcen");
        }
    }

    private void AddAttackSpeed()
    {
        int currentLevel = UnitToEmpower.attackSpeedLevel;
        List<ResourceCost> nextLevelRequirements = CalculateResourceCost(UnitToEmpower.attackSpeedLevel);

        // Überprüfen, ob der Spieler genug Ressourcen hat
        if (PlayerHasEnoughResources(nextLevelRequirements))
        {
            // Ressourcen abziehen
            SubtractResources(nextLevelRequirements);

            // Level erhöhen
            UnitToEmpower.IncreaseAttackSpeed();

            // UI aktualisieren
            UpdateGrid(AttackSpeedUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.attackSpeedLevel));
            //update the Details
            UpdateDetails();
        }
        else
        {
            Debug.Log("Nicht genügend Ressourcen");
        }
    }

    private void AddSpeed()
    {
        int currentLevel = UnitToEmpower.speedLevel;
        List<ResourceCost> nextLevelRequirements = CalculateResourceCost(UnitToEmpower.speedLevel);

        // Überprüfen, ob der Spieler genug Ressourcen hat
        if (PlayerHasEnoughResources(nextLevelRequirements))
        {
            // Ressourcen abziehen
            SubtractResources(nextLevelRequirements);

            // Level erhöhen;
            UnitToEmpower.IncreaseSpeed();

            // UI aktualisieren
            UpdateGrid(SpeedUsedRessourcesPanel, CalculateResourceCost(UnitToEmpower.speedLevel));
            //update the Details
            UpdateDetails();
        }
        else
        {
            Debug.Log("Nicht genügend Ressourcen");
        }
    }

    private void AddHealth()
    {
        int currentLevel = UnitToEmpower.healthLevel;
        List<ResourceCost> nextLevelRequirements = CalculateResourceCost(UnitToEmpower.healthLevel);

        // Überprüfen, ob der Spieler genug Ressourcen hat
        if (PlayerHasEnoughResources(nextLevelRequirements))
        {
            // Ressourcen abziehen
            SubtractResources(nextLevelRequirements);

            // Level erhöhen
            UnitToEmpower.IncreaseMaxHealth();

            // UI aktualisieren
            UpdateGrid(HealthUsedRessourcesPanel,CalculateResourceCost(UnitToEmpower.healthLevel));
            //update the Details
            UpdateDetails();
        }
        else
        {
            Debug.Log("Nicht genügend Ressourcen");
        }
  
    }

    private bool SubtractResources(List<ResourceCost> nextLevelRequirements)
    {
        foreach (ResourceCost cost in nextLevelRequirements)
        {
            if (inventoryManager.GetItemAmountForPlayer(1, cost.resourceName) < cost.cost)
            {
                return false;
            }
            else 
            {
                inventoryManager.RemoveItemForPlayer(1, cost.resourceName);
            }
        }
        return true;
    }

    private bool PlayerHasEnoughResources(List<ResourceCost> nextLevelRequirements)
    {
        foreach (ResourceCost cost in nextLevelRequirements)
        { 
            if(inventoryManager.GetItemAmountForPlayer(1,cost.resourceName) < cost.cost)
            {
                return false;
            }
        }

        return true;
    }


    private void UpdateDetails()
    {
        DetailsPanel.GetComponent<UI_UnitDetails>().UpdateDetails(UnitToEmpower);
    }

    public void SetUnit(Unit unit)
    {
        UnitToEmpower = unit;
        InitilizeGrid();
    }

    private void UpdateResourceAmounts()
    {
        for (int i = 0; i < Resources.Length; i++)
        {
            TextMeshProUGUI text = Resources[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text.name == "Amount")
            {
                DebugManager.Log("corect Text found");
                string itemName = GetItemByIndex(i);
                int amount = inventoryManager.GetItemAmountForPlayer(i, itemName);
                playerResourceAmount[itemName] =amount;
                text.text = FormatHelper.CleanNumber(amount);
            }
            
        }
    }

    private string GetItemByIndex(int index)
    {
        return index switch
        {
            0 => "ashes",
            1 => "glitter_dust",
            2 => "blood",
            _ => null
        };

    }

    private void UpdateGrid(GameObject panel, List<ResourceCost> resCost)
    {
        foreach (Transform child in panel.transform)
        {
            //
            Destroy(child.gameObject);
        }

        // Fügt neue Ressourcen-UI - Elemente hinzu
        foreach (var requirement in resCost)
        {
            // Hier erstellst du ein neues UI-Element für jede Ressource
            // Angenommen, du hast eine Vorlage für Ressourceneinträge (z.B. ein Prefab)
            GameObject resourceEntry = Instantiate(ressEntryPrefab, panel.transform);

            // Setzt das Icon und den Text (Ressourcenname und benötigte Anzahl)
            resourceEntry.GetComponent<Image>().sprite = GetResourceIcon(requirement.resourceName);  // Icon wird gesetzt
            resourceEntry.GetComponentInChildren<TextMeshProUGUI>().text = FormatHelper.CleanNumber(requirement.cost);  // Menge wird gesetzt
        }

    }

    private Sprite GetResourceIcon(string resourceName)
    {
        //TODO
        return null;
    }
}

public class ResourceCost
{
    public string resourceName;
    public int cost;

}



