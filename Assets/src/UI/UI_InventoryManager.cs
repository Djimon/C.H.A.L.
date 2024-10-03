using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UI_InventoryManager : MonoBehaviour
{
    public GameObject inventory;

    public GameObject itemSlotPrefab;
    public Transform contentPanel;

    public Button btnModules;
    public Button btnRemains;
    public Button btnRunes;

    private PlayerInventory playerInventory;

    private EItemType currentItemType;

    private List<GameObject> pooledInventory = new List<GameObject>();
    private int ActiveInventories = 0;

    // Start is called before the first frame update
    void Start()
    {

        playerInventory = inventory.GetComponent<InventoryManager>().GetPlayerinventory(1);


        btnModules.onClick.AddListener(() => ShowItems(EItemType.Module));
        btnRemains.onClick.AddListener(() => ShowItems(EItemType.Remains));
        btnRunes.onClick.AddListener(() => ShowItems(EItemType.Rune));

        // Standardmäßig Module anzeigen
        ShowItems(EItemType.Module);
    }

    private void ShowItems(EItemType itemType)
    {
        currentItemType = itemType;
        
        for (int i = 0; i < pooledInventory.Count; i++)
        {
            pooledInventory[i].SetActive(false);
        }

        ActiveInventories = 0;

        // Item-Kästchen generieren und hinzufügen
        if (currentItemType == EItemType.Module)
        {
            var container = playerInventory.GetContainer<Module>(currentItemType);
            foreach (var module in container.GetAllItems())
            {
                CreateOrUpdateItemBox(module); // Name des Moduls anzeigen
            }
        }
        else if (currentItemType == EItemType.Remains)
        {
            var container = playerInventory.GetContainer<Remains>(currentItemType);
            foreach (var remains in container.GetAllItems())
            {
                CreateOrUpdateItemBox(remains); // Name des Remains anzeigen
            }
        }
        else if (currentItemType == EItemType.Rune)
        {
            var container = playerInventory.GetContainer<Rune>(currentItemType);
            foreach (var rune in container.GetAllItems())
            {
                CreateOrUpdateItemBox(rune); // Name der Rune anzeigen
            }
        }
    }

    private void CreateOrUpdateItemBox(ItemSlot itemSlot)
    {
        GameObject newItem;

        if (ActiveInventories < pooledInventory.Count)
        {
            newItem = pooledInventory[ActiveInventories];
            newItem.SetActive(true);
        }
        else
        {
            newItem = Instantiate(itemSlotPrefab, contentPanel);
            pooledInventory.Add(newItem);
        }

        UpdateItemBoc(itemSlot, newItem);

        ActiveInventories++;
    }

    private static void UpdateItemBoc(ItemSlot itemSlot, GameObject newItem)
    {
        // Finde die Image-Komponente im Prefab
        Image itemImage = newItem.GetComponentInChildren<Image>();
        if (itemImage != null && itemSlot.Item != null)
        {
            // Setze das Bild für das Item (falls du ein Sprite hast)
            itemImage.sprite = itemSlot.Item.Image; // Beispiel: itemSprite ist ein Sprite in der Item-Klasse
        }

        // Finde die Text-Komponenten anhand des Namens der GameObjects
        Text[] texts = newItem.GetComponentsInChildren<Text>();

        foreach (Text text in texts)
        {
            if (text.name == "ItemName")
            {
                // Setze den Item-Namen
                text.text = LocalisationManager.GetTranslation(itemSlot.Item.Name);
            }
            else if (text.name == "ItemAmount")
            {
                // Setze die Anzahl des Items
                text.text = itemSlot.Amount.ToString();
            }
        }
    }
}


