using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LanguageSelector : MonoBehaviour
{
    public GameObject languageDropdownObject;
    public Button btnConfirm;

    private Button btnToggle;

    [SerializeField]
    public TMP_Dropdown languageDropDown;
    // Start is called before the first frame update
    private void Awake()
    {
        languageDropDown = languageDropdownObject.GetComponent<TMP_Dropdown>();
        btnToggle = this.GetComponent<Button>();
    }

    void Start()
    {
        // Populate the dropdown with the language enum values
        PopulateLanguageDropdown();
        languageDropDown.value = (int)LocalisationManager.currentLanguage;

        // Add listener to the confirm button
        btnConfirm.onClick.AddListener(OnConfirmLanguageSelection);
        btnToggle.onClick.AddListener(OnToggleLanguageMenue);

        languageDropdownObject.SetActive(false);
    }

    private void OnToggleLanguageMenue()
    {
        languageDropdownObject.SetActive(!languageDropdownObject.activeSelf);
    }

    void PopulateLanguageDropdown()
    {
        // Clear existing options
        languageDropDown.ClearOptions();

        // Get all the enum values and convert them to strings
        List<string> languageOptions = new List<string>();

        foreach (var language in Enum.GetValues(typeof(ELanguages)))
        {
            languageOptions.Add(language.ToString());
        }

        // Add the language options to the dropdown
        languageDropDown.AddOptions(languageOptions);
    }

    void OnConfirmLanguageSelection()
    {
        // Get the selected dropdown value
        int selectedIndex = languageDropDown.value;
        ELanguages selectedLanguage = (ELanguages)selectedIndex;

        // Set the language in the TranslationManager
        LocalisationManager.ChangeLanguage(selectedLanguage);

        OnToggleLanguageMenue();
    }
}
