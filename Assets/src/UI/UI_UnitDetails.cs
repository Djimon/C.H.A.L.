using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitDetails : MonoBehaviour
{
    public GameObject SelectionPanel;
    public GameObject EmpowerPanel;
    
    public Button btnEmpower;
    public Button btnSubmit;
    public Button btnExit;

    public TextMeshProUGUI monsterID;
    public TextMeshProUGUI monsterName;

    public TextMeshProUGUI monsterHealth;
    public TextMeshProUGUI monsterPower;
    public TextMeshProUGUI monsterSpeed;
    public TextMeshProUGUI monsterAttackSpeed;

    public List<Button> runes;

    private Unit selectedUnit;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        btnEmpower.onClick.AddListener(OpenEmpowerPanel);
        btnSubmit.onClick.AddListener(SubmitSelection);
        btnExit.onClick.AddListener(Close);
        
    }

    private void SubmitSelection()
    {
        SelectionPanel.GetComponent<UI_UnitSelection>().AddSelectedUnitToSlot();
        Close();
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void OpenEmpowerPanel()
    {
        EmpowerPanel.SetActive(true);
        EmpowerPanel.GetComponent<UI_EmpowerUnit>().SetUnit(selectedUnit);
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        UpdateText();
    }

    internal void UpdateDetails(Unit unitToEmpower)
    {
        selectedUnit = unitToEmpower;
        UpdateText();
    }

    private void UpdateText()
    {
        monsterID.text = selectedUnit.MonsterData.monsterID.ToString("D3");
        monsterName.text = selectedUnit.MonsterData.monsterName;
        monsterHealth.text = "Health: " + FormatHelper.CleanNumber(selectedUnit.MaxHealth);
        monsterPower.text = "Power: " + FormatHelper.CleanNumber(selectedUnit.AttackPower);
        monsterSpeed.text = "Speed: " + FormatHelper.CleanNumber(selectedUnit.WalkingSpeed);
        monsterAttackSpeed.text = "Attack Speed: " + FormatHelper.CleanNumber(selectedUnit.AttackSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
