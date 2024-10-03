using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] menus; // Array von Menüs


    // Methode zum Öffnen eines Menüs
    public void OpenMenu(GameObject menu)
    {
        // Schließe alle Menüs
        foreach (GameObject m in menus)
        {
            m.SetActive(false);
        }
        // Öffne das angegebene Menü
        menu.SetActive(true);
    }

    // Methode zum Schließen aller Menüs
    public void CloseAllMenus()
    {
        foreach (GameObject m in menus)
        {
            m.SetActive(false);
        }
    }
}
