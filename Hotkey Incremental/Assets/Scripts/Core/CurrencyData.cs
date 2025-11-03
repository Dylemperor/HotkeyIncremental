
using System.Collections.Generic;

public class CurrencyData
{
    public string letter;
    public double amount;

    public Dictionary<string, UpgradeData> upgrades = new Dictionary<string, UpgradeData>();
    public bool isUnlocked;

    public CurrencyData(string letter)
    {
        this.letter = letter;
        amount = 0;
        InitializeDefaultUpgrades();
    }

    public void InitializeDefaultUpgrades()
    {
        // BaseProduction: adds +1 to base production per level
        upgrades["BaseProduction"] = new UpgradeData("Base Production", 1, 1, 25, 
            new int[] { 5, 20, 50, 100, 250, 500 }, // Phase thresholds: level 5, level 20, level 50
            new double[] { 1.1, 1.15, 1.25, 1.35, 1.45, 1.5 }, // Multipliers: +10%, +25%, +100%
            true, 1.0); // Additive effect, +1 per level
        
        // Multiplier: adds +0.5 to multiplier per level
        upgrades["Multiplier"] = new UpgradeData("Multiplier", 1, 1, 500, 
            new int[] { 5, 10, 25,50 }, // Phase thresholds
            new double[] { 1.25, 1.5, 1.75,2 }, // Multipliers
            true, 0.5); // Additive effect, +0.5 per level 
        
        upgrades["Exponent"] = new UpgradeData(
            "Exponent", 1, 1, 10000, 10, true, 0.1);
        
        // Next-letter upgrades keep canonical keys; names are pretty for UI
        upgrades["nextLetterBaseProduction"] = new UpgradeData(
            "Next Letter Base Production", 1, 1, 10000000000, 100, true, 1);
        upgrades["nextLetterMulti"] = new UpgradeData(
            "Next Letter Multi", 1, 1, 10000000000000, 1000, true, 0.5);
        upgrades["nextLetterExponent"] = new UpgradeData(
            "Next Letter Exponent", 1, 1, 10000000000000000, 10000, true, 0.1);
    }

    public double GetMultiplier()
    {
        return upgrades["BaseProduction"].effect;
    }
    public double GetExponent()
    {
        return upgrades["Exponent"].effect;
    }
}