using System.Collections.Generic;

public class NumberData
{
    public int number;
    public double amount;
    
    public Dictionary<string, UpgradeData> upgrades = new Dictionary<string, UpgradeData>();
    public bool isUnlocked;
    
    public NumberData(int number)
    {
        this.number = number;
        amount = 0;
        isUnlocked = false;
        InitializeDefaultUpgrades();
    }
    
    public void InitializeDefaultUpgrades()
    {
        if (number == 1)
        {
            // Number 1 upgrades: Row multipliers
            // Effect stores the multiplier value (2.0 or 5.0), but only applies when level > 1
            upgrades["TopRow2x"] = new UpgradeData("Top Row 2x", 1, 2.0, 1, 1.0, false, 0);
            upgrades["MiddleRow2x"] = new UpgradeData("Middle Row 2x", 1, 2.0, 1, 1.0, false, 0);
            upgrades["BottomRow2x"] = new UpgradeData("Bottom Row 2x", 1, 2.0, 1, 1.0, false, 0);
            upgrades["TopRow5x"] = new UpgradeData("Top Row 5x", 1, 5.0, 3, 1.0, false, 0);
            upgrades["MiddleRow5x"] = new UpgradeData("Middle Row 5x", 1, 5.0, 3, 1.0, false, 0);
            upgrades["BottomRow5x"] = new UpgradeData("Bottom Row 5x", 1, 5.0, 3, 1.0, false, 0);
        }
        else if (number == 2)
        {
            // Number 2 upgrades: First Half Automation
            upgrades["FirstHalfAutomation"] = new UpgradeData("First Half Automation", 1, 1.0, 5, 1.0, false, 0);
        }
        else if (number == 3)
        {
            // Number 3 upgrades: Second Half Automation
            upgrades["SecondHalfAutomation"] = new UpgradeData("Second Half Automation", 1, 1.0, 5, 1.0, false, 0);
        }
        else if (number >= 4 && number <= 6)
        {
            // Numbers 4-6: Global production multipliers based on Number resets
            // These will be implemented with effect based on total resets
            double baseMultiplier = number == 4 ? 2.0 : (number == 5 ? 3.0 : 5.0);
            upgrades[$"GlobalMultiplier{number}"] = new UpgradeData($"Global Multiplier {number}x", 1, baseMultiplier, 
                number * 10, 1.0, false, 0);
        }
        else if (number >= 7 && number <= 9)
        {
            // Numbers 7-9: Exponential bonuses
            double baseBonus = number == 7 ? 1.5 : (number == 8 ? 2.0 : 3.0);
            upgrades[$"ExponentialBonus{number}"] = new UpgradeData($"Exponential Bonus {baseBonus}x", 1, baseBonus, 
                number * 50, 1.0, false, 0);
        }
    }
}

