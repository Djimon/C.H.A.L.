using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenInventory : MonoBehaviour
{
    // Referenz auf das UI-Men�
    public GameObject menuPanel;

    // Referenz auf den Button
    public Button toggleButton;

    void Start()
    {
        // Initial: Men� versteckt
        menuPanel.SetActive(false);

        // Button-Event hinzuf�gen
        toggleButton.onClick.AddListener(ToggleMenuVisibility);
    }

    // Funktion zum Umschalten des Men�s
    void ToggleMenuVisibility()
    {
        // Toggle: aktiviert/deaktiviert
        menuPanel.SetActive(!menuPanel.activeSelf);
    }
}
