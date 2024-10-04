using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    public GameObject unitSelectionPanel;
    public GameObject unitDetailsPanel;
    public List<Unit> availableUnits;
    public Transform availableUnitsGrid;
    public List<Button> playerSlots;
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


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < playerSlots.Count; i++)
        {
            int index = i; // Wichtig: Lokale Kopie der Schleifenvariable f�r den Listener!
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
        unitSelectionPanel.SetActive(true); // Zeigt das Auswahlpanel an
    }

    private void PopulateAvailableUnitsGrid()
    {
        // Bestimme die Anzahl der freigeschalteten Slots
        int unlockedSlots = GetUnlockedSlotCount(currentPlayerLever);

        // Z�hle die Anzahl der freigeschalteten Slots pro Gr��e
        int availableSmallSlots = CountAvailableSlotsForSize(EUnitSize.small, unlockedSlots);
        int availableMediumSlots = CountAvailableSlotsForSize(EUnitSize.medium, unlockedSlots);
        int availableLargeSlots = CountAvailableSlotsForSize(EUnitSize.large, unlockedSlots);


        foreach (Unit unit in availableUnits)
        {
            GameObject unitButton = new GameObject("UnitButton"); // Button erstellen.
            Button btn = unitButton.AddComponent<Button>(); // Button Komponente hinzuf�gen.

            // Das 3D-Modell der Einheit als UI-Element rendern (hier vielleicht ein 2D-Bild anstelle des Modells).
            // Hier k�nnte man z.B. eine RenderTexture verwenden, um das 3D-Modell darzustellen.
            Image unitImage = unitButton.AddComponent<Image>();
            unitImage.sprite = unit.unit3DModelScreenShot;

            //for later "D3" makes the number allways 3 members, with leading 0s
            string IDAndName = unit.MonsterData.monsterID.ToString("D3") + " - " + unit.MonsterData.monsterName;
            DebugManager.Log($"populated: {IDAndName} to available Units.",2,"UI");

            btn.onClick.AddListener(() => OnUnitSelected(unit));

            if ((unit.UnitSize == EUnitSize.large && (largeUnitsSelected >= maxLargeUnits || availableLargeSlots == 0)) ||
            (unit.UnitSize == EUnitSize.medium && (mediumUnitsSelected >= maxMediumUnits || availableMediumSlots == 0)) ||
            (unit.UnitSize == EUnitSize.small && selectedUnits.Count >= maxTotalUnits) ||
            (selectedUnits.Count >= unlockedSlots)) // Sicherstellen, dass nur so viele Einheiten ausgew�hlt werden wie Slots verf�gbar sind.
            {
                btn.interactable = false; // Ausgrauen, wenn nicht ausw�hlbar.
                unitImage.color = new UnityEngine.Color(1, 1, 1, 0.5f);
            }

            unitButton.transform.SetParent(availableUnitsGrid, false);
        }
    }

    private int CountAvailableSlotsForSize(EUnitSize size, int unlockedSlots)
    {
        EUnitSize[] currentSlotConfig = slotConfigurations[currentSlotLayoutLevel]; // Hole das aktuelle Slot-Layout

        int availableSlots = 0;

        // Durchlaufe die Slots und z�hle, wie viele f�r die aktuelle Einheitengr��e freigeschaltet sind
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
            AddSelectedUnitToSlot(unit);
        }
    }

    private void LoadUnitDetails(Unit unit)
    {
        //TODO:
        //Stats auslesne und anzeigen
        //Runen level pr�fen und setzen
        
    }

    private void AddSelectedUnitToSlot(Unit unit)
    {
        selectedUnits.Add(unit);
        AssignUnitToSlot(unit);

        if (unit.UnitSize == EUnitSize.large)
            largeUnitsSelected++;
        else if (unit.UnitSize == EUnitSize.medium)
            mediumUnitsSelected++;
        else
            smallUnitsSelected++;

        // Schlie�e das Auswahlfenster
        unitSelectionPanel.SetActive(false);
    }

    private void AssignUnitToSlot(Unit unit)
    {
        EUnitSize[] currentSlotConfig = slotConfigurations[currentSlotLayoutLevel]; // Aktuelles Layout

        // Sicherstellen, dass der Slot-Index g�ltig ist und die Einheit zur Slot-Gr��e passt
        if (currentSlotIndex >= 0 && currentSlotConfig[currentSlotIndex] == unit.UnitSize)
        {
            // Setzt das 3D-Modell-Bild in den entsprechenden Slot
            playerSlots[currentSlotIndex].GetComponentInChildren<Image>().sprite = unit.unit3DModelScreenShot;

            // Slot-Index zur�cksetzen und UnitSelectionPanel schlie�en
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
                playerSlots[i].GetComponent<Image>().sprite = lockedSlot[i]; // Vorh�ngeschloss.
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
        return unlockedSlotCount; // Gibt die Anzahl der freigeschalteten Slots zur�ck.
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
            // Starte das Spiel mit den ausgew�hlten Einheiten
            DebugManager.Log("Game Started with selected units",2);
            // Hier die Logik, um den GameState zu �ndern.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
