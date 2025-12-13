using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NumberView : MonoBehaviour
{
    public NumberManager numberManager;
    public NumberPrestigeManager numberPrestigeManager;
    
    [Header("Number Tabs")]
    public Button[] numberTabButtons; // 9 buttons for numbers 1-9
    public GameObject[] numberTabPanels; // 9 panels, one for each number
    
    [Header("Current Number Display")]
    public TMP_Text currentNumberDisplayText;
    
    [Header("Converter")]
    public Button convertButton;
    public TMP_Text convertButtonText;
    
    [Header("Upgrades")]
    public TMP_Text[] upgradeTexts; // 6 upgrade displays per number
    public Button[] upgradeButtons; // 6 upgrade buttons per number
    
    [Header("Update Settings")]
    public float updateInterval = 0.1f;
    
    private int currentNumberTab = 1; // Currently selected number (1-9)
    
    private void Start()
    {
        // Initially hide all panels
        for (int i = 0; i < numberTabPanels.Length; i++)
        {
            if (numberTabPanels[i] != null)
                numberTabPanels[i].SetActive(false);
        }
        
        // Show first number tab if unlocked
        ShowNumberTab(1);
        
        // Setup tab buttons
        for (int i = 0; i < numberTabButtons.Length && i < 9; i++)
        {
            int number = i + 1;
            if (numberTabButtons[i] != null)
            {
                numberTabButtons[i].onClick.AddListener(() => ShowNumberTab(number));
                numberTabButtons[i].gameObject.SetActive(false); // Hidden until unlocked
            }
        }
        
        StartCoroutine(UpdateNumberView());
    }
    
    public void ShowNumberTab(int number)
    {
        if (number < 1 || number > 9)
            return;
            
        // Hide all panels
        for (int i = 0; i < numberTabPanels.Length; i++)
        {
            if (numberTabPanels[i] != null)
                numberTabPanels[i].SetActive(false);
        }
        
        // Show selected panel
        if (number - 1 < numberTabPanels.Length && numberTabPanels[number - 1] != null)
        {
            numberTabPanels[number - 1].SetActive(true);
        }
        
        currentNumberTab = number;
        RefreshCurrentNumberUI();
    }
    
    private IEnumerator UpdateNumberView()
    {
        while (true)
        {
            UpdateTabVisibility();
            RefreshCurrentNumberUI();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void UpdateTabVisibility()
    {
        for (int i = 0; i < numberTabButtons.Length && i < 9; i++)
        {
            int number = i + 1;
            if (numberTabButtons[i] != null && numberManager != null)
            {
                bool hasCurrency = numberManager.GetNumberAmount(number) > 0 || numberManager.allNumbers[number].isUnlocked;
                numberTabButtons[i].gameObject.SetActive(hasCurrency);
            }
        }
    }
    
    private void RefreshCurrentNumberUI()
    {
        if (numberManager == null || !numberManager.allNumbers.ContainsKey(currentNumberTab))
            return;
            
        var numberData = numberManager.allNumbers[currentNumberTab];
        
        // Update number display
        if (currentNumberDisplayText != null)
        {
            currentNumberDisplayText.text = $"{NumberFormatter.Format(numberData.amount)} {currentNumberTab}s";
        }
        
        // Update converter button
        if (convertButton != null && convertButtonText != null)
        {
            bool canConvert = currentNumberTab < 9 && numberData.amount >= 10;
            convertButton.interactable = canConvert;
            
            if (currentNumberTab < 9)
            {
                convertButtonText.text = $"Convert 10 {currentNumberTab}s → 1 {currentNumberTab + 1}";
            }
            else
            {
                convertButtonText.text = "Max Number Reached";
                convertButton.interactable = false;
            }
        }
        
        // Update upgrades
        RefreshUpgradeUI(numberData, currentNumberTab);
    }
    
    private void RefreshUpgradeUI(NumberData numberData, int number)
    {
        if (upgradeTexts == null || upgradeButtons == null)
            return;
            
        List<string> upgradeKeys = new List<string>(numberData.upgrades.Keys);
        
        for (int i = 0; i < upgradeTexts.Length && i < upgradeButtons.Length; i++)
        {
            if (i < upgradeKeys.Count)
            {
                string upgradeKey = upgradeKeys[i];
                var upgrade = numberData.upgrades[upgradeKey];
                
                // Update upgrade text
                if (upgradeTexts[i] != null)
                {
                    bool isPurchased = upgrade.level > 1;
                    string statusText = isPurchased ? "✓ Purchased" : $"Cost: {upgrade.cost} {number}s";
                    upgradeTexts[i].text = $"{upgrade.name}\n{statusText}";
                }
                
                // Update upgrade button
                if (upgradeButtons[i] != null)
                {
                    bool canAfford = numberData.amount >= upgrade.cost && !isPurchased;
                    upgradeButtons[i].interactable = canAfford;
                    upgradeButtons[i].gameObject.SetActive(!isPurchased);
                }
            }
            else
            {
                // Hide unused upgrade slots
                if (upgradeTexts[i] != null)
                    upgradeTexts[i].gameObject.SetActive(false);
                if (upgradeButtons[i] != null)
                    upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void OnConvertButtonClick()
    {
        if (numberManager != null && currentNumberTab < 9)
        {
            bool success = numberManager.ConvertNumber(currentNumberTab, currentNumberTab + 1);
            if (success)
            {
                RefreshCurrentNumberUI();
            }
        }
    }
    
    public void OnUpgradeButtonClick(int upgradeIndex)
    {
        if (numberManager == null || !numberManager.allNumbers.ContainsKey(currentNumberTab))
            return;
            
        var numberData = numberManager.allNumbers[currentNumberTab];
        List<string> upgradeKeys = new List<string>(numberData.upgrades.Keys);
        
        if (upgradeIndex >= 0 && upgradeIndex < upgradeKeys.Count)
        {
            string upgradeKey = upgradeKeys[upgradeIndex];
            var upgrade = numberData.upgrades[upgradeKey];
            
            if (numberData.amount >= upgrade.cost && upgrade.level == 1)
            {
                numberData.amount -= upgrade.cost;
                upgrade.Upgrade();
                
                // Special handling for automation upgrades
                if (upgradeKey == "FirstHalfAutomation" || upgradeKey == "SecondHalfAutomation")
                {
                    // Automation upgrade just needs to be purchased, enabling is separate
                }
                
                RefreshCurrentNumberUI();
            }
        }
    }
}

