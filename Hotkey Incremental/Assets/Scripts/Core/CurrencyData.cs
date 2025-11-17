
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
            new int[] { 5, 25, 75, 125, 250, 500 }, // Phase thresholds: level 5, level 25, etc.
            new double[] { 1.08, 1.1, 1.125, 1.15, 1.2, 2 }, // Multipliers: +10%, +15%, etc.
            true, 1.0); // Additive effect, +1 per level
        
        // Multiplier: adds +0.5 to multiplier per level
        upgrades["Multiplier"] = new UpgradeData("Multiplier", 1, 1, 500, 
            new int[] { 5, 15, 50, 100, 500 }, // Phase thresholds
            new double[] { 1.15, 1.175, 1.2, 1.35, 2.5 }, // Multipliers
            true, 0.5); // Additive effect, +0.5 per level 
        
        upgrades["Exponent"] = new UpgradeData(
            "Exponent", 1, 1, 10000, 
            new int[] { 5, 10, 20 , 50, 100, 500 }, 
            new double[] { 2.5, 2.75, 3, 3.5, 5, 10 }, 
            true, 0.1); // Additive effect, +0.1 per level
        
        // Next-letter upgrades keep canonical keys; names are pretty for UI
        upgrades["nextLetterBaseProduction"] = new UpgradeData(
            "Next Letter Base Production", 1, 1, 100000000000, 5, true, 1);
        upgrades["nextLetterMulti"] = new UpgradeData(
            "Next Letter Multi", 1, 1, 1000000000000000, 25, true, 0.5);
        upgrades["nextLetterExponent"] = new UpgradeData(
            "Next Letter Exponent", 1, 1, 1000000000000000000, 50, true, 0.1);
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