using UnityEngine;
using System;

public class NumberPrestigeManager : MonoBehaviour
{
    public PrestigeManager prestigeManager;
    public CurrencyManager currencyManager;
    public NumberManager numberManager;
    public UnlockManager unlockManager;
    
    private int totalNumberResets = 0;
    
    // Check if Number reset is available (26 gold plates)
    public bool CanPerformNumberReset()
    {
        if (prestigeManager == null)
            return false;
            
        return prestigeManager.GetGoldPlatedCount() >= 26;
    }
    
    // Perform Number reset
    public void PerformNumberReset()
    {
        if (!CanPerformNumberReset())
            return;
            
        // Get Z amount before reset for currency calculation
        double zAmount = 0;
        if (currencyManager.allLetters.ContainsKey("Z"))
        {
            zAmount = currencyManager.allLetters["Z"].amount;
        }
        
        // Calculate Number 1 currency gained
        // Formula: floor(log10(max(Z_amount, 1e15) / 1e15)) + 1, minimum 1
        double baseThreshold = 1e15; // 1sx
        double normalizedAmount = Math.Max(zAmount, baseThreshold);
        double logValue = Math.Log10(normalizedAmount / baseThreshold);
        int number1Gained = Math.Max(1, (int)Math.Floor(logValue) + 1);
        
        // Reset all letter currencies to 0 (including A)
        ResetAllCurrencies();
        
        // Reset all upgrades for all letters
        ResetAllUpgrades();
        
        // Reset ALL prestige multipliers (silver/gold)
        ResetAllPrestigeMultipliers();
        
        // Lock all letters except 'A'
        LockAllLettersExceptA();
        
        // Ensure letter A amount is 0 after reset
        if (currencyManager.allLetters.ContainsKey("A"))
        {
            currencyManager.allLetters["A"].amount = 0;
        }
        
        // Grant Number 1 currency
        if (numberManager != null)
        {
            numberManager.AddNumber(1, number1Gained);
        }
        
        totalNumberResets++;
        
        Debug.Log($"Number Reset: Gained {number1Gained} Number 1 currency (from {zAmount} Z). Total resets: {totalNumberResets}");
    }
    
    // Lock all letters except 'A'
    private void LockAllLettersExceptA()
    {
        if (currencyManager == null || currencyManager.allLetters == null)
        {
            Debug.LogError("NumberPrestigeManager: CurrencyManager or allLetters is null!");
            return;
        }
        
        foreach (var pair in currencyManager.allLetters)
        {
            if (pair.Key != "A")
            {
                pair.Value.isUnlocked = false;
            }
            else
            {
                pair.Value.isUnlocked = true;
            }
        }
    }
    
    // Reset all upgrades for all letters
    private void ResetAllUpgrades()
    {
        if (currencyManager == null || currencyManager.allLetters == null)
            return;
        
        foreach (var pair in currencyManager.allLetters)
        {
            var data = pair.Value;
            data.InitializeDefaultUpgrades();
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
    
    public int GetTotalNumberResets()
    {
        return totalNumberResets;
    }
    
    // Save Number prestige data
    public void SaveNumberPrestigeData()
    {
        PlayerPrefs.SetInt("NumberPrestige_TotalResets", totalNumberResets);
        PlayerPrefs.Save();
    }
    
    // Load Number prestige data
    public void LoadNumberPrestigeData()
    {
        if (PlayerPrefs.HasKey("NumberPrestige_TotalResets"))
        {
            totalNumberResets = PlayerPrefs.GetInt("NumberPrestige_TotalResets", 0);
        }
    }
    
    // Reset all Number prestige data (called on hard reset)
    public void ResetAllNumberPrestigeData()
    {
        totalNumberResets = 0;
        
        PlayerPrefs.DeleteKey("NumberPrestige_TotalResets");
        PlayerPrefs.Save();
    }
    
    private void Start()
    {
        LoadNumberPrestigeData();
    }
    
    private void OnApplicationQuit()
    {
        SaveNumberPrestigeData();
    }
}

