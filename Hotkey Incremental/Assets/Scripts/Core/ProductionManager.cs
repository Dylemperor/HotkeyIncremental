using UnityEngine;
using System.Collections;
using System;

public class ProductionManager : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public float productionInterval = 0.1f; // How often to calculate production
    
    private DateTime lastUpdateTime;
    private bool isFirstUpdate = true;
    
    private void Start()
    {
        lastUpdateTime = DateTime.Now;
        StartCoroutine(ProductionLoop());
    }
    
    private void Update()
    {
        // Check if we need to catch up on production (e.g., tab was in background)
        // This ensures production continues even when the coroutine is throttled
        if (!isFirstUpdate)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastUpdate = currentTime - lastUpdateTime;
            
            // If more than 2x the production interval has passed, we likely missed updates
            // Trigger a catch-up production calculation
            if (timeSinceLastUpdate.TotalSeconds > productionInterval * 2)
            {
                CalculateAndAddProduction();
            }
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        // When the application regains focus, immediately catch up on production
        if (hasFocus && !isFirstUpdate)
        {
            CalculateAndAddProduction();
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // When unpausing, catch up on production
        if (!pauseStatus && !isFirstUpdate)
        {
            CalculateAndAddProduction();
        }
    }
    
    private IEnumerator ProductionLoop()
    {
        while (true)
        {
            CalculateAndAddProduction();
            yield return new WaitForSeconds(productionInterval);
        }
    }
    
    private double CalculateProduction(CurrencyData data)
    {
        // Get upgrade values
        double baseProduction = data.upgrades["BaseProduction"].effect;
        double multiplier = data.upgrades["Multiplier"].effect;
        double exponent = data.upgrades["Exponent"].effect;
        
        // Calculate production: base * multiplier * exponent
        // Exponent acts as an additional multiplier that increases with upgrades
        double production = Math.Pow(baseProduction * multiplier, exponent);
        
        return production;
    }
    
    private void CalculateAndAddProduction()
    {
        DateTime currentTime = DateTime.Now;
        double elapsedSeconds = productionInterval; // Default to normal interval
        
        // Calculate actual elapsed time since last update
        // This ensures production continues even when the tab is in the background
        if (!isFirstUpdate)
        {
            TimeSpan timeSinceLastUpdate = currentTime - lastUpdateTime;
            elapsedSeconds = timeSinceLastUpdate.TotalSeconds;
            
            // Cap the maximum elapsed time to prevent huge jumps (e.g., max 1 hour offline)
            // This prevents exploits and keeps the game balanced
            double maxOfflineTime = 7200.0; // 2 hours in seconds
            if (elapsedSeconds > maxOfflineTime)
            {
                elapsedSeconds = maxOfflineTime;
            }
            
            // If elapsed time is negative or too small, use the normal interval
            if (elapsedSeconds < 0 || elapsedSeconds < productionInterval * 0.5)
            {
                elapsedSeconds = productionInterval;
            }
        }
        else
        {
            isFirstUpdate = false;
        }
        
        // Update the last update time
        lastUpdateTime = currentTime;
        
        foreach (var pair in currencyManager.allLetters)
        {
            if (pair.Value.isUnlocked)
            {
                double production = CalculateProduction(pair.Value);
                currencyManager.AddCurrency(pair.Key, production * elapsedSeconds);
                
                // Calculate and add next letter production
                double nextLetterProduction = CalculateNextLetterProduction(pair.Value);
                if (nextLetterProduction > 0)
                {
                    string nextLetter = GetNextLetter(pair.Key);
                    if (!string.IsNullOrEmpty(nextLetter))
                    {
                        currencyManager.AddCurrency(nextLetter, nextLetterProduction * elapsedSeconds);
                    }
                }
            }
        }
    }
    
    private double CalculateNextLetterProduction(CurrencyData data)
    {
        // Get next letter upgrade values
        double nextLetterBaseProduction = data.upgrades["nextLetterBaseProduction"].effect;
        double nextLetterMulti = data.upgrades["nextLetterMulti"].effect;
        double nextLetterExponent = data.upgrades["nextLetterExponent"].effect;
        
        // Calculate next letter production: base * multiplier * exponent
        // Exponent acts as an additional multiplier that increases with upgrades
        double production = nextLetterBaseProduction * nextLetterMulti * nextLetterExponent;
        
        return production;
    }
    
    private string GetNextLetter(string currentLetter)
    {
        if (currentLetter.Length == 1 && currentLetter[0] >= 'A' && currentLetter[0] < 'Z')
        {
            return ((char)(currentLetter[0] + 1)).ToString();
        }
        return null;
    }
    
    // Method to get production rate for UI display
    public double GetProductionRate(string letter)
    {
        if (currencyManager.allLetters.ContainsKey(letter))
        {
            var data = currencyManager.allLetters[letter];
            if (data.isUnlocked)
            {
                return CalculateProduction(data);
            }
        }
        return 0;
    }
    
    // Method to get next letter production rate for UI display
    public double GetNextLetterProductionRate(string letter)
    {
        if (currencyManager.allLetters.ContainsKey(letter))
        {
            var data = currencyManager.allLetters[letter];
            if (data.isUnlocked)
            {
                return CalculateNextLetterProduction(data);
            }
        }
        return 0;
    }
}
