
using System.Collections.Generic;

public class LetterData
{
    public string letter;
    public double amount;

    public Dictionary<string, UpgradeData> upgrades = new Dictionary<string, UpgradeData>();

    public LetterData(string letter)
    {
        this.letter = letter;
        amount = 0;
        InitializeDefaultUpgrades();
    }

    public void InitializeDefaultUpgrades()
    {
        upgrades["BaseProduction"] = new UpgradeData("BaseProduction", 1, 1, 100, 1.25);
        upgrades["Multiplier"] = new UpgradeData("Multiplier", 1, 1, 1000,10);
        upgrades["Exponent"] = new UpgradeData("Exponent", 1, 1, 10000000,100);

        upgrades["nextLetterBaseProduction"] = new UpgradeData("nextLetterBaseProduction", 1, 1, 1000000000000, 100);
        upgrades["nextLetterMulti"] = new UpgradeData("nextLetterMulti", 1, 1, 1000000000000000,1000);
        upgrades["nextLetterExponent"] = new UpgradeData("nextLetterExponent", 1, 1, 1000000000000000000,10000);
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