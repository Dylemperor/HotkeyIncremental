
using System.Collections.Generic;

public class CurrencyData
{
    public string letter;
    public double amount;

    public Dictionary<string, UpgradeData> upgrades = new Dictionary<string, UpgradeData>();
    public bool isUnlocked;
    
    // Prestige system
    public bool isSilver = false;
    public bool isGold = false;
    public double prestigeMultiplier = 1.0; // Combined multiplier from silver/gold prestige

    public CurrencyData(string letter)
    {
        this.letter = letter;
        amount = 0;
        InitializeDefaultUpgrades();
    }

    public void InitializeDefaultUpgrades()
    {
        // Determine if this is letter A or a later letter for scaling adjustment
        bool isLetterA = letter == "A";
        double scalingMultiplier = isLetterA ? 1.0 : 1.15; // 15% harsher scaling for letters after A
        
        // BaseProduction: adds +1 to base production per level
        double[] baseProductionMultipliers = isLetterA 
            ? new double[] { 1.08,  1.125, 1.15, 1.2, 15 } // Original for A
            : new double[] { 1.12, 1.15, 1.2, 1.2, 20 }; // Harsher for B-Z
        
        upgrades["BaseProduction"] = new UpgradeData("Base Production", 1, 1, 25, 
            new int[] { 10, 50, 100, 250, 500, 1000 }, 
            baseProductionMultipliers, 
            true, 1.0); 
        
        // Multiplier: adds +1 to multiplier per level
        double[] multiplierMultipliers = isLetterA
            ? new double[] { 1.15, 1.175, 1.2, 1.35, 2.5, 15 } // Original for A
            : new double[] { 1.2, 1.3, 1.4, 1.5, 3.0, 25 }; // Harsher for B-Z
        
        upgrades["Multiplier"] = new UpgradeData("Multiplier", 1, 1, 500, 
            new int[] { 10, 25, 50, 100, 500, 1000 }, // Phase thresholds
            multiplierMultipliers, // Multipliers adjusted for letter position
            true, 1); // Additive effect, +1 per level 
        
        // Exponent: adds +0.1 to exponent per level
        double[] exponentMultipliers = isLetterA
            ? new double[] { 2.4, 2.75, 3, 3.5, 5, 50 } // Original for A
            : new double[] { 2.5, 2.9, 3.25, 4, 5.25, 60 }; // Harsher for B-Z
        
        upgrades["Exponent"] = new UpgradeData(
            "Exponent", 1, 1, 10000, 
            new int[] { 5, 10, 20 , 50, 100, 200}, 
            exponentMultipliers, 
            true, 0.1); 
        
        // Next-letter upgrades
        // Start with effect = 0 so first purchase adds the increment (not 1 + increment)
        // Next-letter upgrades also scale harsher after A
        double[] nextBaseMultipliers = isLetterA
            ? new double[] {2, 2.5, 3, 5, 25}
            : new double[] {2.25, 2.7, 3.25, 5, 20};
        
        double[] nextMultiMultipliers = isLetterA
            ? new double[] {3.5, 5, 7.5, 10, 40}
            : new double[] {4, 6, 9, 10, 50};
        
        double[] nextExpMultipliers = isLetterA
            ? new double[] {7.5, 10, 25, 250}
            : new double[] {10, 15, 40, 500};
        
        upgrades["nextLetterBaseProduction"] = new UpgradeData(
            "Next Letter Base Production", 1, 0, 50000000000, 
            new int[] { 10, 20, 25, 50, 100}, nextBaseMultipliers, true, 1);
        upgrades["nextLetterMulti"] = new UpgradeData(
            "Next Letter Multi", 1, 0, 1000000000000000, 
            new int[] { 10, 20, 25, 50, 100}, nextMultiMultipliers, true, 1);
        upgrades["nextLetterExponent"] = new UpgradeData(
            "Next Letter Exponent", 1, 0, 1000000000000000000, 
            new int[] { 10, 20, 25, 50}, nextExpMultipliers, true, 0.1);
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