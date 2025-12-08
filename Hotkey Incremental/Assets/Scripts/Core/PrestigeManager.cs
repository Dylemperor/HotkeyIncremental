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
    
    // Check if gold prestige is available (all letters are silver plated)
    public bool CanGoldPrestige()
    {
        // Check if all letters are silver plated
        int totalUnlocked = currencyManager.allLetters.Values.Count(l => l.isUnlocked);
        return totalUnlocked > 0 && silverPlatedCount >= totalUnlocked;
    }
    
    // Perform silver prestige reset
    public void PerformSilverPrestige()
    {
        if (!CanSilverPrestige())
            return;
        
        // Get the next letter to silver plate (in unlock order)
        string letterToPlate = GetNextLetterToSilverPlate();
        if (letterToPlate == null)
            return;
        
        // Reset all letter currencies
        ResetAllCurrencies();
        
        // Lock all letters except 'A'
        LockAllLettersExceptA();
        
        // Apply silver plating to the next letter
        var letterData = currencyManager.allLetters[letterToPlate];
        letterData.isSilver = true;
        letterData.prestigeMultiplier *= silverMultiplier;
        silverPlatedCount++;
        
        Debug.Log($"Silver Prestige: {letterToPlate} is now silver plated! ({silverPlatedCount}/{GetTotalUnlockedLetters()} letters plated)");
    }
    
    // Lock all letters except 'A'
    private void LockAllLettersExceptA()
    {
        foreach (var pair in currencyManager.allLetters)
        {
            if (pair.Key != "A")
            {
                pair.Value.isUnlocked = false;
            }
            else
            {
                // Ensure 'A' remains unlocked
                pair.Value.isUnlocked = true;
            }
        }
        
        // Update UI to reflect locked letters
        if (letterSelector != null)
        {
            letterSelector.UpdateAllLetterButtons();
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
        
        // Reset all letter currencies
        ResetAllCurrencies();
        
        // Lock all letters except 'A'
        LockAllLettersExceptA();
        
        // Apply gold plating to the next letter (overrides silver)
        var letterData = currencyManager.allLetters[letterToPlate];
        
        // If it was silver, decrement silver count
        if (letterData.isSilver)
        {
            silverPlatedCount--;
        }
        
        letterData.isSilver = false; // Gold replaces silver
        letterData.isGold = true;
        
        // Remove silver multiplier and apply gold multiplier
        // If it was silver, divide by silver multiplier first, then multiply by gold
        if (letterData.prestigeMultiplier >= silverMultiplier)
        {
            letterData.prestigeMultiplier = (letterData.prestigeMultiplier / silverMultiplier) * goldMultiplier;
        }
        else
        {
            letterData.prestigeMultiplier = goldMultiplier;
        }
        
        goldPlatedCount++;
        
        Debug.Log($"Gold Prestige: {letterToPlate} is now gold plated! ({goldPlatedCount}/{GetTotalUnlockedLetters()} letters plated)");
    }
    
    // Get the next letter to silver plate (in unlock order)
    private string GetNextLetterToSilverPlate()
    {
        foreach (string letter in unlockManager.letterOrder)
        {
            if (currencyManager.allLetters.ContainsKey(letter))
            {
                var data = currencyManager.allLetters[letter];
                if (data.isUnlocked && !data.isSilver && !data.isGold)
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
                if (data.isUnlocked && data.isSilver && !data.isGold)
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
        foreach (var pair in currencyManager.allLetters)
        {
            pair.Value.amount = 0;
        }
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

