using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenInventory : MonoBehaviour
{
    // Referenz auf das UI-Menü
    public GameObject menuPanel;

    // Referenz auf den Button
    public Button toggleButton;

    void Start()
    {
        // Initial: Menü versteckt
        menuPanel.SetActive(false);

        // Button-Event hinzufügen
        toggleButton.onClick.AddListener(ToggleMenuVisibility);
    }

    // Funktion zum Umschalten des Menüs
    void ToggleMenuVisibility()
    {
        // Toggle: aktiviert/deaktiviert
        menuPanel.SetActive(!menuPanel.activeSelf);
    }
}
