using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PrestigeManager : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public UnlockManager unlockManager;
    public LetterSelector letterSelector; // Optional: for UI updates
    
    [Header("Silver Prestige Settings")]
    public double silverPrestigeRequirement = 100000000000; // 100 Billion Z currency
    public double silverMultiplier = 2.0; // 2x boost per silver prestige
    
    [Header("Gold Prestige Settings")]
    public double goldMultiplier = 5.0; // 5x boost per gold prestige
    
    // Track which letters have been silver/gold plated
    private int silverPlatedCount = 0;
    private int goldPlatedCount = 0;
    
    private void Start()
    {
        // Wait a frame to ensure CurrencyManager is initialized
        StartCoroutine(InitializePrestige());
    }
    
    private System.Collections.IEnumerator InitializePrestige()
    {
        // Wait for CurrencyManager to be ready
        yield return null;
        
        // Load prestige data if available
        LoadPrestigeData();
    }
    
    // Check if silver prestige is available (Z currency >= 100 Billion)
    public bool CanSilverPrestige()
    {
        if (!currencyManager.allLetters.ContainsKey("Z"))
            return false;
            
        var zData = currencyManager.allLetters["Z"];
        return zData.isUnlocked && zData.amount >= silverPrestigeRequirement;
    }
    
    // Check if gold prestige is available (all 26 letters are silver plated)
    public bool CanGoldPrestige()
    {
        // Check if all 26 letters (A-Z) are silver plated
        return silverPlatedCount >= 26;
    }
    
    // Perform silver prestige reset
    public void PerformSilverPrestige()
    {
        if (!CanSilverPrestige())
            return;
        
        // Get the next letter to silver plate (in unlock order) BEFORE locking
        string letterToPlate = GetNextLetterToSilverPlate();
        if (letterToPlate == null)
            return;
        
        // Save current plating status before reset
        var savedPlating = new Dictionary<string, (bool isSilver, bool isGold, double multiplier)>();
        foreach (var pair in currencyManager.allLetters)
        {
            savedPlating[pair.Key] = (pair.Value.isSilver, pair.Value.isGold, pair.Value.prestigeMultiplier);
        }
        
        // Reset all letter currencies to 0 (including A)
        ResetAllCurrencies();
        
        // Reset all upgrades for all letters
        ResetAllUpgrades();
        
        // Lock all letters except 'A'
        LockAllLettersExceptA();
        
        // Ensure letter A amount is 0 after reset
        if (currencyManager.allLetters.ContainsKey("A"))
        {
            currencyManager.allLetters["A"].amount = 0;
        }
        
        // Restore all previously plated letters' multipliers
        foreach (var pair in savedPlating)
        {
            var letterData = currencyManager.allLetters[pair.Key];
            if (pair.Value.isGold)
            {
                letterData.isGold = true;
                letterData.isSilver = false;
                letterData.prestigeMultiplier = goldMultiplier;
            }
            else if (pair.Value.isSilver)
            {
                letterData.isSilver = true;
                letterData.isGold = false;
                letterData.prestigeMultiplier = silverMultiplier;
            }
        }
        
        // Apply silver plating to the next letter (will override if already set above, but that's fine)
        var letterData = currencyManager.allLetters[letterToPlate];
        letterData.isSilver = true;
        letterData.isGold = false; // Silver takes precedence over gold only for this new letter
        letterData.prestigeMultiplier = silverMultiplier;
        silverPlatedCount++;
        
        // Update UI AFTER applying plating so colors are correct
        UpdateLetterSelectorUI();
        
        Debug.Log($"Silver Prestige: {letterToPlate} is now silver plated! ({silverPlatedCount}/26 letters plated)");
    }
    
    // Lock all letters except 'A'
    private void LockAllLettersExceptA()
    {
        if (currencyManager == null || currencyManager.allLetters == null)
        {
            Debug.LogError("PrestigeManager: CurrencyManager or allLetters is null!");
            return;
        }
        
        int lockedCount = 0;
        foreach (var pair in currencyManager.allLetters)
        {
            if (pair.Key != "A")
            {
                pair.Value.isUnlocked = false;
                lockedCount++;
            }
            else
            {
                // Ensure 'A' remains unlocked
                pair.Value.isUnlocked = true;
            }
        }
        
        Debug.Log($"PrestigeManager: Locked {lockedCount} letters, kept 'A' unlocked.");
    }
    
    // Reset all upgrades for all letters
    private void ResetAllUpgrades()
    {
        if (currencyManager == null || currencyManager.allLetters == null)
            return;
        
        foreach (var pair in currencyManager.allLetters)
        {
            var data = pair.Value;
            // Reinitialize upgrades to reset them to default state
            data.InitializeDefaultUpgrades();
        }
    }
    
    // Update LetterSelector UI
    private void UpdateLetterSelectorUI()
    {
        if (letterSelector != null)
        {
            letterSelector.UpdateAllLetterButtons();
            Debug.Log("PrestigeManager: LetterSelector UI updated after prestige.");
        }
        else
        {
            Debug.LogWarning("PrestigeManager: LetterSelector is not assigned! UI will not update after prestige.");
        }
    }
    
    // Perform gold prestige reset
    public void PerformGoldPrestige()
    {
        if (!CanGoldPrestige())
            return;
        
        // Get the next letter to gold plate (in unlock order)
        string letterToPlate = GetNextLetterToGoldPlate();
        if (letterToPlate == null)
            return;
        
        // Save current plating status before reset
        var savedPlating = new Dictionary<string, (bool isSilver, bool isGold, double multiplier)>();
        foreach (var pair in currencyManager.allLetters)
        {
            savedPlating[pair.Key] = (pair.Value.isSilver, pair.Value.isGold, pair.Value.prestigeMultiplier);
        }
        
        // Reset all letter currencies to 0 (including A)
        ResetAllCurrencies();
        
        // Reset all upgrades for all letters
        ResetAllUpgrades();
        
        // Lock all letters except 'A'
        LockAllLettersExceptA();
        
        // Ensure letter A amount is 0 after reset
        if (currencyManager.allLetters.ContainsKey("A"))
        {
            currencyManager.allLetters["A"].amount = 0;
        }
        
        // Restore all previously plated letters' multipliers
        // Gold plates are preserved, silver plates that aren't being converted stay silver
        foreach (var pair in savedPlating)
        {
            var letterData = currencyManager.allLetters[pair.Key];
            if (pair.Value.isGold)
            {
                letterData.isGold = true;
                letterData.isSilver = false;
                letterData.prestigeMultiplier = goldMultiplier;
            }
            else if (pair.Value.isSilver && pair.Key != letterToPlate)
            {
                // Keep silver plating for letters not being converted to gold
                letterData.isSilver = true;
                letterData.isGold = false;
                letterData.prestigeMultiplier = silverMultiplier;
            }
        }
        
        // Apply gold plating to the next letter (converts silver to gold)
        var letterData = currencyManager.allLetters[letterToPlate];
        letterData.isSilver = false; // Gold replaces silver
        letterData.isGold = true;
        letterData.prestigeMultiplier = goldMultiplier;
        goldPlatedCount++;
        
        // Update UI AFTER applying plating so colors are correct
        UpdateLetterSelectorUI();
        
        Debug.Log($"Gold Prestige: {letterToPlate} is now gold plated! ({goldPlatedCount}/26 letters plated)");
    }
    
    // Get the next letter to silver plate (in unlock order)
    private string GetNextLetterToSilverPlate()
    {
        foreach (string letter in unlockManager.letterOrder)
        {
            if (currencyManager.allLetters.ContainsKey(letter))
            {
                var data = currencyManager.allLetters[letter];
                // Plate letters in order, regardless of current unlock status
                // (they will be locked after prestige anyway, except A)
                if (!data.isSilver && !data.isGold)
                {
                    return letter;
                }
            }
        }
        return null;
    }
    
    // Get the next letter to gold plate (in unlock order)
    private string GetNextLetterToGoldPlate()
    {
        foreach (string letter in unlockManager.letterOrder)
        {
            if (currencyManager.allLetters.ContainsKey(letter))
            {
                var data = currencyManager.allLetters[letter];
                // Plate letters in order that are silver but not gold
                if (data.isSilver && !data.isGold)
                {
                    return letter;
                }
            }
        }
        return null;
    }
    
    // Reset all letter currencies to 0
    private void ResetAllCurrencies()
    {
        if (currencyManager == null || currencyManager.allLetters == null)
            return;
        
        foreach (var pair in currencyManager.allLetters)
        {
            pair.Value.amount = 0;
        }
    }
    
    // Reset all prestige multipliers to 1.0 and clear plating status
    private void ResetAllPrestigeMultipliers()
    {
        if (currencyManager == null || currencyManager.allLetters == null)
            return;
        
        foreach (var pair in currencyManager.allLetters)
        {
            pair.Value.prestigeMultiplier = 1.0;
            pair.Value.isSilver = false;
            pair.Value.isGold = false;
        }
        
        silverPlatedCount = 0;
        goldPlatedCount = 0;
    }
    
    // Public method to reset all prestige data (called on full game reset)
    public void ResetAllPrestigeData()
    {
        ResetAllPrestigeMultipliers();
        
        // Also clear prestige PlayerPrefs
        foreach (var pair in currencyManager.allLetters)
        {
            string letter = pair.Key;
            PlayerPrefs.DeleteKey($"Prestige_Silver_{letter}");
            PlayerPrefs.DeleteKey($"Prestige_Gold_{letter}");
            PlayerPrefs.DeleteKey($"Prestige_Multiplier_{letter}");
        }
        
        PlayerPrefs.DeleteKey("Prestige_SilverCount");
        PlayerPrefs.DeleteKey("Prestige_GoldCount");
        PlayerPrefs.Save();
    }
    
    // Get total number of unlocked letters
    private int GetTotalUnlockedLetters()
    {
        return currencyManager.allLetters.Values.Count(l => l.isUnlocked);
    }
    
    // Get count of silver plated letters
    public int GetSilverPlatedCount()
    {
        return silverPlatedCount;
    }
    
    // Get count of gold plated letters
    public int GetGoldPlatedCount()
    {
        return goldPlatedCount;
    }
    
    // Save prestige data
    public void SavePrestigeData()
    {
        // Save silver/gold status for each letter
        foreach (var pair in currencyManager.allLetters)
        {
            string letter = pair.Key;
            var data = pair.Value;
            
            PlayerPrefs.SetInt($"Prestige_Silver_{letter}", data.isSilver ? 1 : 0);
            PlayerPrefs.SetInt($"Prestige_Gold_{letter}", data.isGold ? 1 : 0);
            PlayerPrefs.SetString($"Prestige_Multiplier_{letter}", data.prestigeMultiplier.ToString());
        }
        
        PlayerPrefs.SetInt("Prestige_SilverCount", silverPlatedCount);
        PlayerPrefs.SetInt("Prestige_GoldCount", goldPlatedCount);
        PlayerPrefs.Save();
    }
    
    // Load prestige data
    private void LoadPrestigeData()
    {
        silverPlatedCount = 0;
        goldPlatedCount = 0;
        
        foreach (var pair in currencyManager.allLetters)
        {
            string letter = pair.Key;
            var data = pair.Value;
            
            if (PlayerPrefs.HasKey($"Prestige_Silver_{letter}"))
            {
                data.isSilver = PlayerPrefs.GetInt($"Prestige_Silver_{letter}") == 1;
                if (data.isSilver) silverPlatedCount++;
            }
            
            if (PlayerPrefs.HasKey($"Prestige_Gold_{letter}"))
            {
                data.isGold = PlayerPrefs.GetInt($"Prestige_Gold_{letter}") == 1;
                if (data.isGold) goldPlatedCount++;
            }
            
            if (PlayerPrefs.HasKey($"Prestige_Multiplier_{letter}"))
            {
                string multStr = PlayerPrefs.GetString($"Prestige_Multiplier_{letter}");
                if (double.TryParse(multStr, out double mult))
                {
                    data.prestigeMultiplier = mult;
                }
            }
        }
    }
    
    private void OnApplicationQuit()
    {
        SavePrestigeData();
    }
}

