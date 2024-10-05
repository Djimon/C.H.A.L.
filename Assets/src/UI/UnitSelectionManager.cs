using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    public GameObject unitSelectionPanel;
    public GameObject unitDetailsPanel;
    public GameObject unitEmpowerPanel;
    public List<Unit> availableUnits;
    public Transform availableUnitsGrid;
    public List<Button> playerSlots;
    public Button btnEmpower;
    public Button btnSelectUnit;
    public Button btnStart;
    public List<Sprite> lockedSlot;
    public List<Button> runeSlots;

    private int largeUnitsSelected = 0;
    private int smallUnitsSelected = 0;
    private int mediumUnitsSelected = 0;

    private List<Unit> selectedUnits = new List<Unit>();

    private const int maxLargeUnits = 1;
    private const int maxMediumUnits = 3;
    private const int maxTotalUnits = 5;

    private List<EUnitSize[]> slotConfigurations = new List<EUnitSize[]>
    {
        new EUnitSize[]{ EUnitSize.small, EUnitSize.small, EUnitSize.medium, EUnitSize.small, EUnitSize.small},
        new EUnitSize[]{ EUnitSize.small, EUnitSize.medium, EUnitSize.large, EUnitSize.medium, EUnitSize.small},
    };

    [SerializeField]
    private int currentPlayerLever = 0;
    private List<int> unlockLevels = new List<int> {0,3,6,9,15};
    private int currentSlotLayoutLevel = 0;
    private int currentSlotIndex;
    private Unit loadedUnit;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerSlots.Count; i++)
        {
            int index = i; // Wichtig: Lokale Kopie der Schleifenvariable für den Listener!
            playerSlots[i].onClick.AddListener(() => OnPlayerSlotClicked(index));
        }

        PopulateAvailableUnitsGrid();

        btnStart.onClick.AddListener(StartGame);
        DebugManager.Log($"added logik to the Statbutton {btnStart.name}",2,"Info");

        unitSelectionPanel.SetActive(false);

        UnlockSlots();
    }

    private void OnPlayerSlotClicked(int index)
    {
        currentSlotIndex = index; // Speichert den Index des angeklickten Slots
        PurgeOldGrid();
        PopulateAvailableUnitsGrid();
        unitSelectionPanel.SetActive(true); // Zeigt das Auswahlpanel an
    }

    private void PurgeOldGrid()
    { 
        Button[] unitbuttons = availableUnitsGrid.GetComponentsInChildren<Button>();
        if (unitbuttons == null)
        {
            return;
        }

        for (int i = 0; i < unitbuttons.Length; i++)
        { 
            if(unitbuttons[i].name == "UnitButton")
            {
                GameObject.Destroy(unitbuttons[i].gameObject);
            }
        }
        
    }

    private void PopulateAvailableUnitsGrid()
    {
        // Bestimme die Anzahl der freigeschalteten Slots
        int unlockedSlots = GetUnlockedSlotCount(currentPlayerLever);

        // Zähle die Anzahl der freigeschalteten Slots pro Größe
        int availableSmallSlots = CountAvailableSlotsForSize(EUnitSize.small, unlockedSlots);
        int availableMediumSlots = CountAvailableSlotsForSize(EUnitSize.medium, unlockedSlots);
        int availableLargeSlots = CountAvailableSlotsForSize(EUnitSize.large, unlockedSlots);


        foreach (Unit unit in availableUnits)
        {
            GameObject unitButton = new GameObject("UnitButton"); // Button erstellen.
            Button btn = unitButton.AddComponent<Button>(); // Button Komponente hinzufügen.

            // Das 3D-Modell der Einheit als UI-Element rendern (hier vielleicht ein 2D-Bild anstelle des Modells).
            // Hier könnte man z.B. eine RenderTexture verwenden, um das 3D-Modell darzustellen.
            Image unitImage = unitButton.AddComponent<Image>();
            unitImage.sprite = unit.unit3DModelScreenShot;

            //for later "D3" makes the number allways 3 members, with leading 0s
            string IDAndName = unit.MonsterData.monsterID.ToString("D3") + " - " + unit.MonsterData.monsterName;
            DebugManager.Log($"populated: {IDAndName} to available Units.",2,"UI");

            btn.onClick.AddListener(() => OnUnitSelected(unit));

            if ((unit.UnitSize == EUnitSize.large && (largeUnitsSelected >= maxLargeUnits || availableLargeSlots == 0)) ||
            (unit.UnitSize == EUnitSize.medium && (mediumUnitsSelected >= maxMediumUnits || availableMediumSlots == 0)) ||
            (unit.UnitSize == EUnitSize.small && selectedUnits.Count >= maxTotalUnits) ||
            (selectedUnits.Count >= unlockedSlots)) // Sicherstellen, dass nur so viele Einheiten ausgewählt werden wie Slots verfügbar sind.
            {
                btn.interactable = false; // Ausgrauen, wenn nicht auswählbar.
                unitImage.color = new UnityEngine.Color(1, 1, 1, 0.5f);
            }

            unitButton.transform.SetParent(availableUnitsGrid, false);
        }
    }

    private int CountAvailableSlotsForSize(EUnitSize size, int unlockedSlots)
    {
        EUnitSize[] currentSlotConfig = slotConfigurations[currentSlotLayoutLevel]; // Hole das aktuelle Slot-Layout

        int availableSlots = 0;

        // Durchlaufe die Slots und zähle, wie viele für die aktuelle Einheitengröße freigeschaltet sind
        for (int i = 0; i < unlockedSlots; i++)
        {
            if (currentSlotConfig[i] == size)
            {
                availableSlots++;
            }
        }

        return availableSlots;
    }

    private void OnUnitSelected(Unit unit)
    {
        if (selectedUnits.Count < maxTotalUnits)
        {
            if (unit.UnitSize == EUnitSize.large && largeUnitsSelected >= maxLargeUnits)
                return;
            if (unit.UnitSize == EUnitSize.medium && mediumUnitsSelected >= maxMediumUnits)
                return;

            //TODO: First Open Detail-Panel in the Detailpanel there is an "Submit" button afterwards add selected unit
            LoadUnitDetails(unit);
            unitDetailsPanel.SetActive(true);
            
        }
    }

    private void LoadUnitDetails(Unit unit)
    {
        loadedUnit = unit;
        Button[] btns = unitDetailsPanel.GetComponentsInChildren<Button>();
        Button btnSubmit = null;
        Button btnEmpower = null;
        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i].name == "btnSubmit")
            { 
                btnSubmit = btns[i];   
            }
            if (btns[i].name == "btnEmpower")
            {
                btnEmpower = btns[i];
            }
        }

        btnEmpower.onClick.AddListener(OpenEmpowerMenu);
        btnSubmit.onClick.AddListener(AddSelectedUnitToSlot);

        //TODO:
        //Stats auslesne und anzeigen
        UpdateUnitStats();
    
        //Runen level prüfen und setzen


    }

    //Move to own script within the Detaisl-Panel
    public void UpdateUnitStats()
    {
        // Texte der item.stats anpassen
    }

    private void OpenEmpowerMenu()
    {
        unitEmpowerPanel.SetActive(true);
        unitEmpowerPanel.GetComponent<UIEmpowerUnit>().SetUnit(loadedUnit);
    }

    private void AddSelectedUnitToSlot()
    {
        selectedUnits.Add(loadedUnit);
        AssignUnitToSlot(loadedUnit);

        if (loadedUnit.UnitSize == EUnitSize.large)
            largeUnitsSelected++;
        else if (loadedUnit.UnitSize == EUnitSize.medium)
            mediumUnitsSelected++;
        else
            smallUnitsSelected++;

        availableUnits.Remove(loadedUnit);

        // Schließe das Auswahlfenster 
        unitDetailsPanel.SetActive(false);
        unitEmpowerPanel.SetActive(false);
        unitSelectionPanel.SetActive(false);
    }

    private void AssignUnitToSlot(Unit unit)
    {
        EUnitSize[] currentSlotConfig = slotConfigurations[currentSlotLayoutLevel]; // Aktuelles Layout

        // Sicherstellen, dass der Slot-Index gültig ist und die Einheit zur Slot-Größe passt
        if (currentSlotIndex >= 0 && currentSlotConfig[currentSlotIndex] == unit.UnitSize)
        {
            // Setzt das 3D-Modell-Bild in den entsprechenden Slot
            playerSlots[currentSlotIndex].GetComponentInChildren<Image>().sprite = unit.unit3DModelScreenShot;

            // Slot-Index zurücksetzen und UnitSelectionPanel schließen
            currentSlotIndex = -1;
            unitSelectionPanel.SetActive(false);
        }

    }

    private void UnlockSlots()
    {
        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < GetUnlockedSlotCount(currentPlayerLever))
            {
                playerSlots[i].interactable = true; // Slot freigeschaltet.
                playerSlots[i].GetComponent<Image>().sprite = null; // Kein Schloss, freier Slot.
            }
            else
            {
                playerSlots[i].interactable = false; // Slot gesperrt.
                playerSlots[i].GetComponent<Image>().sprite = lockedSlot[i]; // Vorhängeschloss.
            }
        }
    }

    private int GetUnlockedSlotCount(int level)
    {
        int unlockedSlotCount = 0;
        foreach (int unlockLevel in unlockLevels)
        {
            if (level >= unlockLevel)
            {
                unlockedSlotCount++;
            }
        }
        return unlockedSlotCount; // Gibt die Anzahl der freigeschalteten Slots zurück.
    }

    public void LevelUpSlots(int newLevel)
    {
        currentPlayerLever = newLevel;
        if(currentPlayerLever >= 20)
        {
            currentSlotLayoutLevel = 1; //use 2nd Layout
        }
        
        UnlockSlots();
    }

    private void StartGame()
    {
        if (selectedUnits.Count > 0)
        {
            // Starte das Spiel mit den ausgewählten Einheiten
            DebugManager.Log("Game Started with selected units",2);
            // Hier die Logik, um den GameState zu ändern.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
