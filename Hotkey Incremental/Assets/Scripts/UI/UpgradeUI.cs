using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public ProductionManager productionManager;
    public TMP_Text[] upgradeTexts; // Assign upgrade display texts in inspector (should be 6)
    public Button[] upgradeButtons; // Assign upgrade buttons in inspector (should be 6)
    public string currentLetter = "A";
    
    private string[] upgradeNames = {
        "BaseProduction",
        "Multiplier", 
        "Exponent",
        "nextLetterBaseProduction",
        "nextLetterMulti",
        "nextLetterExponent"
    };
    
    private void Start()
    {
        RefreshUpgradeUI();
    }
    
    public void SetCurrentLetter(string letter)
    {
        currentLetter = letter;
        RefreshUpgradeUI();
    }
    
    public void RefreshUpgradeUI()
    {
        if (!currencyManager.allLetters.ContainsKey(currentLetter))
            return;
            
        var currencyData = currencyManager.allLetters[currentLetter];
        
        // Update upgrade texts and buttons for all 6 upgrades using array order
        for (int i = 0; i < upgradeNames.Length; i++)
        {
            if (!currencyData.upgrades.ContainsKey(upgradeNames[i]))
                continue;
            var upgrade = currencyData.upgrades[upgradeNames[i]];
            
            if (i < upgradeTexts.Length && upgradeTexts[i] != null)
            {
                UpdateUpgradeText(upgradeTexts[i], upgradeNames[i], upgrade);
            }
            if (i < upgradeButtons.Length && upgradeButtons[i] != null)
            {
                UpdateUpgradeButton(upgradeButtons[i], upgrade);
            }
        }
    }
    
    private void UpdateUpgradeText(TMP_Text text, string upgradeName, UpgradeData upgradeData)
    {
        if (text != null)
        {
            string displayName = upgradeName;
            string effectDescription = upgradeData.effect.ToString("F1");
            
            // Make the upgrade names more descriptive
            if (upgradeName == "nextLetterBaseProduction")
            {
                displayName = "Next Letter Base Production";
                effectDescription = upgradeData.effect.ToString("F1") + "/s";
            }
            else if (upgradeName == "nextLetterMulti")
            {
                displayName = "Next Letter Multiplier";
            }
            else if (upgradeName == "nextLetterExponent")
            {
                displayName = "Next Letter Power Multiplier";
            }
            else if (upgradeName == "Exponent")
            {
                displayName = "Power Multiplier";
            }
            else if (upgradeName == "BaseProduction")
            {
                displayName = "Base Production";
            }
            
            text.text = $"{displayName}\nLevel: {upgradeData.level}\nCost: {NumberFormatter.Format(upgradeData.cost)}\nEffect: {effectDescription}";
        }
    }
    
    private void UpdateUpgradeButton(Button button, UpgradeData upgradeData)
    {
        if (button != null)
        {
            var currencyData = currencyManager.allLetters[currentLetter];
            button.interactable = currencyData.amount >= upgradeData.cost;
        }
    }
    
    // Function to attach to the "Max All Upgrades" button
    public void MaxAllUpgrades()
    {
        Debug.Log($"MaxAllUpgrades called for letter: {currentLetter}");
        
        if (!currencyManager.allLetters.ContainsKey(currentLetter))
        {
            Debug.LogError($"Letter {currentLetter} not found in currency manager!");
            return;
        }
            
        var currencyData = currencyManager.allLetters[currentLetter];
        bool purchasedAny = false;
        int totalPurchased = 0;
        
        // Try to purchase each upgrade as many times as possible
        foreach (string upgradeName in upgradeNames)
        {
            if (currencyData.upgrades.ContainsKey(upgradeName))
            {
                var upgrade = currencyData.upgrades[upgradeName];
                int purchasedThisUpgrade = 0;
                
                // Keep buying until we can't afford it
                while (currencyData.amount >= upgrade.cost)
                {
                    currencyData.amount -= upgrade.cost;
                    upgrade.Upgrade();
                    purchasedAny = true;
                    purchasedThisUpgrade++;
                }
                
                if (purchasedThisUpgrade > 0)
                {
                    Debug.Log($"Purchased {purchasedThisUpgrade} levels of {upgradeName}");
                }
            }
        }
        
        if (purchasedAny)
        {
            RefreshUpgradeUI();
            Debug.Log($"Maxed all upgrades for letter {currentLetter}. Total purchases: {totalPurchased}");
        }
        else
        {
            Debug.Log($"No upgrades could be purchased for letter {currentLetter} (insufficient funds)");
        }
    }
    
    // Functions to attach to buttons in the inspector
    public void PurchaseBaseProduction()
    {
        PurchaseUpgrade("BaseProduction");
    }
    
    public void PurchaseMultiplier()
    {
        PurchaseUpgrade("Multiplier");
    }
    
    public void PurchaseExponent()
    {
        PurchaseUpgrade("Exponent");
    }
    
    public void PurchaseNextLetterBaseProduction()
    {
        PurchaseUpgrade("nextLetterBaseProduction");
    }
    
    public void PurchaseNextLetterMulti()
    {
        PurchaseUpgrade("nextLetterMulti");
    }
    
    public void PurchaseNextLetterExponent()
    {
        PurchaseUpgrade("nextLetterExponent");
    }
    
    private void PurchaseUpgrade(string upgradeName)
    {
        if (currencyManager.allLetters.ContainsKey(currentLetter))
        {
            var currencyData = currencyManager.allLetters[currentLetter];
            if (currencyData.upgrades.ContainsKey(upgradeName))
            {
                var upgrade = currencyData.upgrades[upgradeName];
                
                if (currencyData.amount >= upgrade.cost)
                {
                    currencyData.amount -= upgrade.cost;
                    upgrade.Upgrade();
                    
                    RefreshUpgradeUI();
                    Debug.Log($"Purchased {upgradeName} for {currentLetter}");
                }
            }
        }
    }
    
    // Method to update all buttons (call this periodically)
    public void UpdateAllButtons()
    {
        RefreshUpgradeUI();
    }
} 