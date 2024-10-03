using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Laboratory : MonoBehaviour
{
    public Button btnExit;

    private void Awake()
    {
        
    }

    private void Start()
    {

        btnExit.onClick.AddListener(() => OnExit());
    }

    private void OnExit()
    {
        this.gameObject.SetActive(false);
    }
}
