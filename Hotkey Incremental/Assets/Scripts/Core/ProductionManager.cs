using UnityEngine;
using System.Collections;
using System;

public class ProductionManager : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public float productionInterval = 0.1f; // How often to calculate production
    
    private void Start()
    {
        StartCoroutine(ProductionLoop());
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
        foreach (var pair in currencyManager.allLetters)
        {
            if (pair.Value.isUnlocked)
            {
                double production = CalculateProduction(pair.Value);
                currencyManager.AddCurrency(pair.Key, production * productionInterval);
                
                // Calculate and add next letter production
                double nextLetterProduction = CalculateNextLetterProduction(pair.Value);
                if (nextLetterProduction > 0)
                {
                    string nextLetter = GetNextLetter(pair.Key);
                    if (!string.IsNullOrEmpty(nextLetter))
                    {
                        currencyManager.AddCurrency(nextLetter, nextLetterProduction * productionInterval);
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
