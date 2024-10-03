using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] menus; // Array von Men�s


    // Methode zum �ffnen eines Men�s
    public void OpenMenu(GameObject menu)
    {
        // Schlie�e alle Men�s
        foreach (GameObject m in menus)
        {
            m.SetActive(false);
        }
        // �ffne das angegebene Men�
        menu.SetActive(true);
    }

    // Methode zum Schlie�en aller Men�s
    public void CloseAllMenus()
    {
        foreach (GameObject m in menus)
        {
            m.SetActive(false);
        }
    }
}
