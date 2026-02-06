using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NumberView : MonoBehaviour
{
    public NumberManager numberManager;
    public NumberPrestigeManager numberPrestigeManager;
    public UIManager uiManager;
    
    [Header("Number Selector Buttons")]
    public Button[] numberButtons; // 9 buttons for numbers 1-9 (similar to LetterSelector)
    
    [Header("Current Number Display")]
    public TMP_Text currentNumberDisplayText; // Shows "5.23 1s" format
    
    [Header("Converter")]
    public Button convertButton;
    public TMP_Text convertButtonText;
    
    [Header("Upgrades")]
    public TMP_Text[] upgradeTexts; // 6 upgrade displays
    public Button[] upgradeButtons; // 6 upgrade buttons
    
    [Header("Update Settings")]
    public float updateInterval = 0.1f;
    
    private int currentNumber = 1; // Currently selected number (1-9)
    
    private void Start()
    {
        // Validate required references
        if (numberManager == null)
        {
            Debug.LogError("NumberView: NumberManager is not assigned!");
            return;
        }
        
        // Setup number selector buttons
        if (numberButtons == null)
        {
            Debug.LogWarning("NumberView: NumberButtons array is not assigned!");
            return;
        }
        
        // Ensure we don't exceed array bounds
        int maxButtons = Mathf.Min(numberButtons.Length, 9);
        
        for (int i = 0; i < maxButtons; i++)
        {
            int number = i + 1;
            if (numberButtons[i] != null)
            {
                int num = number; // Capture for lambda
                numberButtons[i].onClick.AddListener(() => LoadNumber(num));
                numberButtons[i].gameObject.SetActive(false); // Hidden until unlocked
            }
        }
        
        // Initialize with first number, but don't switch views on startup
        currentNumber = 1;
        UpdateNumberInfo();
        
        StartCoroutine(UpdateNumberView());
    }
    
    public void LoadNumber(int number)
    {
        // Validate number range
        if (number < 1 || number > 9)
        {
            Debug.LogWarning($"NumberView: Invalid number {number}. Must be between 1 and 9.");
            return;
        }
        
        if (numberManager == null)
        {
            Debug.LogError("NumberView: Cannot load number - NumberManager is null");
            return;
        }
        
        if (!numberManager.allNumbers.ContainsKey(number))
        {
            Debug.LogWarning($"NumberView: Number {number} not found in NumberManager");
            return;
        }
            
        // Make sure we're on the number page
        if (uiManager != null)
        {
            uiManager.ShowNumberPage();
        }
            
        currentNumber = number;
        UpdateNumberInfo();
    }
    
    private IEnumerator UpdateNumberView()
    {
        while (true)
        {
            UpdateNumberButtonVisibility();
            UpdateNumberInfo();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private void UpdateNumberButtonVisibility()
    {
        if (numberButtons == null || numberManager == null)
            return;
            
        // Ensure we don't exceed array bounds or valid number range
        int maxButtons = Mathf.Min(numberButtons.Length, 9);
        
        for (int i = 0; i < maxButtons; i++)
        {
            int number = i + 1;
            if (numberButtons[i] != null && number >= 1 && number <= 9)
            {
                bool hasCurrency = numberManager.GetNumberAmount(number) > 0 || 
                                  (numberManager.allNumbers.ContainsKey(number) && numberManager.allNumbers[number].isUnlocked);
                numberButtons[i].gameObject.SetActive(hasCurrency);
            }
        }
    }
    
    private void UpdateNumberInfo()
    {
        if (numberManager == null || !numberManager.allNumbers.ContainsKey(currentNumber))
            return;
            
        var numberData = numberManager.allNumbers[currentNumber];
        
        // Update number currency display at top
        if (currentNumberDisplayText != null)
        {
            currentNumberDisplayText.text = $"{NumberFormatter.Format(numberData.amount)} {currentNumber}s";
        }
        
        // Update converter button
        if (convertButton != null && convertButtonText != null)
        {
            bool canConvert = currentNumber < 9 && numberData.amount >= 10;
            convertButton.interactable = canConvert;
            
            if (currentNumber < 9)
            {
                convertButtonText.text = $"Convert 10 {currentNumber}s → 1 {currentNumber + 1}";
            }
            else
            {
                convertButtonText.text = "Max Number Reached";
                convertButton.interactable = false;
            }
        }
        
        // Update upgrades
        RefreshUpgradeUI(numberData, currentNumber);
    }
    
    private void RefreshUpgradeUI(NumberData numberData, int number)
    {
        if (upgradeTexts == null || upgradeButtons == null)
            return;
            
        if (numberData == null || numberData.upgrades == null)
        {
            Debug.LogWarning("NumberView: Invalid NumberData or upgrades dictionary");
            return;
        }
            
        List<string> upgradeKeys = new List<string>(numberData.upgrades.Keys);
        
        // Ensure we don't exceed array bounds
        int maxSlots = Mathf.Min(upgradeTexts.Length, upgradeButtons.Length);
        
        for (int i = 0; i < maxSlots; i++)
        {
            if (i < upgradeKeys.Count)
            {
                string upgradeKey = upgradeKeys[i];
                
                if (!numberData.upgrades.ContainsKey(upgradeKey))
                {
                    Debug.LogWarning($"NumberView: Upgrade key '{upgradeKey}' missing from upgrades dictionary");
                    continue;
                }
                
                var upgrade = numberData.upgrades[upgradeKey];
                
                if (upgrade == null)
                {
                    Debug.LogWarning($"NumberView: Upgrade data is null for key '{upgradeKey}'");
                    continue;
                }
                
                bool isPurchased = upgrade.level > 1;
                
                // Update upgrade text
                if (upgradeTexts[i] != null)
                {
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
        if (numberManager == null)
        {
            Debug.LogWarning("NumberView: Cannot convert - NumberManager is null");
            return;
        }
        
        if (currentNumber >= 9)
        {
            Debug.Log("NumberView: Cannot convert - already at maximum number (9)");
            return;
        }
        
        if (!numberManager.allNumbers.ContainsKey(currentNumber))
        {
            Debug.LogWarning($"NumberView: Current number {currentNumber} not found");
            return;
        }
        
        bool success = numberManager.ConvertNumber(currentNumber, currentNumber + 1);
        if (success)
        {
            UpdateNumberInfo();
        }
        else
        {
            Debug.Log($"NumberView: Conversion failed - insufficient amount (need 10 {currentNumber}s)");
        }
    }
    
    public void OnUpgradeButtonClick(int upgradeIndex)
    {
        if (numberManager == null)
        {
            Debug.LogWarning("NumberView: Cannot purchase upgrade - NumberManager is null");
            return;
        }
        
        if (!numberManager.allNumbers.ContainsKey(currentNumber))
        {
            Debug.LogWarning($"NumberView: Number {currentNumber} not found in NumberManager");
            return;
        }
            
        var numberData = numberManager.allNumbers[currentNumber];
        List<string> upgradeKeys = new List<string>(numberData.upgrades.Keys);
        
        // Validate upgrade index
        if (upgradeIndex < 0 || upgradeIndex >= upgradeKeys.Count)
        {
            Debug.LogWarning($"NumberView: Invalid upgrade index {upgradeIndex} (valid range: 0-{upgradeKeys.Count - 1})");
            return;
        }
        
        string upgradeKey = upgradeKeys[upgradeIndex];
        
        if (!numberData.upgrades.ContainsKey(upgradeKey))
        {
            Debug.LogWarning($"NumberView: Upgrade key '{upgradeKey}' not found");
            return;
        }
        
        var upgrade = numberData.upgrades[upgradeKey];
        
        if (numberData.amount >= upgrade.cost && upgrade.level == 1)
        {
            numberData.amount -= upgrade.cost;
            upgrade.Upgrade();
            
            // Special handling for automation upgrades
            if (upgradeKey == "FirstHalfAutomation" || upgradeKey == "SecondHalfAutomation")
            {
                // Automation upgrade just needs to be purchased, enabling is separate
                Debug.Log($"Automation upgrade '{upgradeKey}' purchased for Number {currentNumber}");
            }
            
            UpdateNumberInfo();
        }
        else if (upgrade.level > 1)
        {
            Debug.Log($"Upgrade '{upgradeKey}' already purchased");
        }
        else
        {
            Debug.Log($"Cannot afford upgrade '{upgradeKey}' - need {upgrade.cost}, have {numberData.amount}");
        }
    }
}

