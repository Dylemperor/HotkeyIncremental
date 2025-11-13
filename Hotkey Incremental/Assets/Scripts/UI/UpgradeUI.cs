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
    
    // Purchase order for MaxAllUpgrades - change this array to change the order
    // Options: "BaseProduction", "Multiplier", "Exponent", 
    //          "nextLetterBaseProduction", "nextLetterMulti", "nextLetterExponent"
    // Set to null to use dynamic ordering (cheapest first)
    private string[] purchaseOrder = {

        "nextLetterExponent",
        "nextLetterMulti",
        "nextLetterBaseProduction",
        "Exponent",
        "Multiplier",
        "BaseProduction"
    };
    
    // Set to true to order by cheapest first, false to use purchaseOrder array
    public bool useCheapestFirstOrder = false;
    
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
                UpdateUpgradeButton(upgradeButtons[i], upgrade, upgradeNames[i]);
            }
        }
    }
    
    private string GetNextLetter(string currentLetter)
    {
        if (string.IsNullOrEmpty(currentLetter) || currentLetter.Length == 0)
            return null;
        
        char currentChar = currentLetter[0];
        if (currentChar >= 'A' && currentChar < 'Z')
        {
            return ((char)(currentChar + 1)).ToString();
        }
        return null; // Z has no next letter
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
                string nextLetter = GetNextLetter(currentLetter);
                if (nextLetter != null)
                {
                    displayName = $"{nextLetter} Base Production";
                }
                else
                {
                    displayName = "Next Letter Base Production (Max)";
                }
                effectDescription = upgradeData.effect.ToString("F1") + "/s";
            }
            else if (upgradeName == "nextLetterMulti")
            {
                string nextLetter = GetNextLetter(currentLetter);
                if (nextLetter != null)
                {
                    displayName = $"{nextLetter} Multiplier";
                }
                else
                {
                    displayName = "Next Letter Multiplier (Max)";
                }
            }
            else if (upgradeName == "nextLetterExponent")
            {
                string nextLetter = GetNextLetter(currentLetter);
                if (nextLetter != null)
                {
                    displayName = $"{nextLetter} Power Multiplier";
                }
                else
                {
                    displayName = "Next Letter Power Multiplier (Max)";
                }
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
    
    private void UpdateUpgradeButton(Button button, UpgradeData upgradeData, string upgradeName)
    {
        if (button != null)
        {
            var currencyData = currencyManager.allLetters[currentLetter];
            bool canAfford = currencyData.amount >= upgradeData.cost;
            
            // Check if this is a next-letter upgrade
            bool isNextLetter = upgradeName == "nextLetterBaseProduction" || 
                                upgradeName == "nextLetterMulti" || 
                                upgradeName == "nextLetterExponent";
            bool isAtMaxLetter = currentLetter == "Z";
            
            // Disable button if it's a next-letter upgrade and we're at Z
            if (isNextLetter && isAtMaxLetter)
            {
                button.interactable = false;
            }
            else
            {
                // Always keep button interactable so it's clickable
                button.interactable = true;
            }
            
            // If disabled (at max letter), use disabled color
            bool isDisabled = isNextLetter && isAtMaxLetter;
            
            Color targetColor;
            if (isDisabled)
            {
                // Fully disabled: very dim grey
                targetColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);
            }
            else if (canAfford)
            {
                // Light up: full opacity and brighter color
                targetColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Mostly Full white = full brightness
            }
            else
            {
                // Slightly greyed out: reduced opacity and dimmed
                targetColor = new Color(0.5f, 0.5f, 0.5f, 0.6f); // Grey with reduced opacity
            }
            
            // Get the button's image component to change visual appearance
            var buttonImage = button.GetComponent<UnityEngine.UI.Image>();
            if (buttonImage != null)
            {
                buttonImage.color = targetColor;
            }
            else
            {
                // If no Image component, try to find it in children
                buttonImage = button.GetComponentInChildren<UnityEngine.UI.Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = targetColor;
                }
            }
            
            // Update all child image components consistently
            var childImages = button.GetComponentsInChildren<UnityEngine.UI.Image>();
            foreach (var childImage in childImages)
            {
                // Skip if it's the same as the button's main image we already handled
                if (childImage == buttonImage) continue;
                
                childImage.color = targetColor;
            }
            
            // Update text color in child text components
            var textComponents = button.GetComponentsInChildren<TMP_Text>();
            foreach (var text in textComponents)
            {
                if (isDisabled)
                {
                    text.color = new Color(0.5f, 0.5f, 0.5f, 0.7f); // Dim grey for disabled
                }
                else if (canAfford)
                {
                    text.color = Color.white;
                }
                else
                {
                    text.color = new Color(0.85f, 0.85f, 0.85f, 1f); // Slightly lighter grey for text readability
                }
            }
            
            // Also update the button's color block to affect default button states
            var colors = button.colors;
            if (isDisabled)
            {
                colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.4f); // Fully disabled
                colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);
                colors.pressedColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);
                colors.selectedColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);
                colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);
            }
            else if (canAfford)
            {
                colors.normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                colors.selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            }
            else
            {
                colors.normalColor = new Color(0.6f, 0.6f, 0.6f, 0.8f); // Greyed out
                colors.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 0.9f);
                colors.pressedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                colors.selectedColor = new Color(0.6f, 0.6f, 0.6f, 0.8f);
            }
            button.colors = colors;
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
        
        // Determine which upgrades to purchase and in what order
        List<string> upgradesToPurchase = new List<string>();
        
        if (useCheapestFirstOrder)
        {
            // Dynamic ordering: sort by cost (cheapest first)
            upgradesToPurchase = GetUpgradesOrderedByCost(currencyData);
        }
        else
        {
            // Use the predefined purchase order
            upgradesToPurchase = new List<string>(purchaseOrder);
        }
        
        // Try to purchase each upgrade as many times as possible in the specified order
        foreach (string upgradeName in upgradesToPurchase)
        {
            // Skip next-letter upgrades if we're at Z
            bool isNextLetter = upgradeName == "nextLetterBaseProduction" || 
                                upgradeName == "nextLetterMulti" || 
                                upgradeName == "nextLetterExponent";
            if (isNextLetter && currentLetter == "Z")
            {
                continue; // Skip next-letter upgrades when at max letter
            }
            
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
            // Prevent purchasing next-letter upgrades when at Z
            bool isNextLetter = upgradeName == "nextLetterBaseProduction" || 
                                upgradeName == "nextLetterMulti" || 
                                upgradeName == "nextLetterExponent";
            if (isNextLetter && currentLetter == "Z")
            {
                Debug.Log($"Cannot purchase {upgradeName} - already at maximum letter (Z)");
                return;
            }
            
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
    
    // Method to get upgrades ordered by cost (cheapest first)
    private List<string> GetUpgradesOrderedByCost(CurrencyData currencyData)
    {
        List<(string name, double cost)> upgradeCosts = new List<(string, double)>();
        
        foreach (string upgradeName in upgradeNames)
        {
            // Skip next-letter upgrades if we're at Z
            bool isNextLetter = upgradeName == "nextLetterBaseProduction" || 
                                upgradeName == "nextLetterMulti" || 
                                upgradeName == "nextLetterExponent";
            if (isNextLetter && currentLetter == "Z")
            {
                continue;
            }
            
            if (currencyData.upgrades.ContainsKey(upgradeName))
            {
                upgradeCosts.Add((upgradeName, currencyData.upgrades[upgradeName].cost));
            }
        }
        
        // Sort by cost (cheapest first)
        upgradeCosts.Sort((a, b) => a.cost.CompareTo(b.cost));
        
        // Return just the names in order
        List<string> orderedNames = new List<string>();
        foreach (var (name, cost) in upgradeCosts)
        {
            orderedNames.Add(name);
        }
        
        return orderedNames;
    }
    
    // Method to update all buttons (call this periodically)
    public void UpdateAllButtons()
    {
        RefreshUpgradeUI();
    }
} 