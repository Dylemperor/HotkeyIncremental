using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NumberManager : MonoBehaviour
{
    public CurrencyManager currencyManager;
    
    // Store Number currencies (1-9)
    public Dictionary<int, NumberData> allNumbers = new Dictionary<int, NumberData>();
    
    // Automation speed upgrade tracking
    public double automationSpeed = 30.0; // Starting speed in seconds
    public int automationSpeedLevel = 0;
    
    private void Start()
    {
        // Initialize all numbers 1-9
        for (int i = 1; i <= 9; i++)
        {
            allNumbers[i] = new NumberData(i);
        }
    }
    
    public double GetNumberAmount(int number)
    {
        if (allNumbers.ContainsKey(number))
            return allNumbers[number].amount;
        return 0;
    }
    
    public void AddNumber(int number, double amount)
    {
        if (allNumbers.ContainsKey(number))
        {
            allNumbers[number].amount += amount;
            if (amount > 0 && !allNumbers[number].isUnlocked)
            {
                allNumbers[number].isUnlocked = true;
            }
        }
    }
    
    public bool ConvertNumber(int fromNumber, int toNumber)
    {
        if (fromNumber < 1 || fromNumber > 8 || toNumber != fromNumber + 1)
            return false;
            
        if (!allNumbers.ContainsKey(fromNumber) || !allNumbers.ContainsKey(toNumber))
            return false;
            
        // Convert 10 of current number to 1 of next number
        if (allNumbers[fromNumber].amount >= 10)
        {
            allNumbers[fromNumber].amount -= 10;
            allNumbers[toNumber].amount += 1;
            
            if (!allNumbers[toNumber].isUnlocked)
            {
                allNumbers[toNumber].isUnlocked = true;
            }
            
            return true;
        }
        return false;
    }
    
    // Get multiplier for a specific keyboard row
    public double GetRowMultiplier(string row)
    {
        double multiplier = 1.0;
        
        // Check Number 1 upgrades for row multipliers
        if (allNumbers.ContainsKey(1) && allNumbers[1].isUnlocked)
        {
            var num1 = allNumbers[1];
            
            if (row == "Top")
            {
                if (num1.upgrades.ContainsKey("TopRow2x") && num1.upgrades["TopRow2x"].level > 1)
                    multiplier *= 2.0; // Fixed 2x multiplier
                if (num1.upgrades.ContainsKey("TopRow5x") && num1.upgrades["TopRow5x"].level > 1)
                    multiplier *= 5.0; // Fixed 5x multiplier (stacks with 2x)
            }
            else if (row == "Middle")
            {
                if (num1.upgrades.ContainsKey("MiddleRow2x") && num1.upgrades["MiddleRow2x"].level > 1)
                    multiplier *= 2.0; // Fixed 2x multiplier
                if (num1.upgrades.ContainsKey("MiddleRow5x") && num1.upgrades["MiddleRow5x"].level > 1)
                    multiplier *= 5.0; // Fixed 5x multiplier (stacks with 2x)
            }
            else if (row == "Bottom")
            {
                if (num1.upgrades.ContainsKey("BottomRow2x") && num1.upgrades["BottomRow2x"].level > 1)
                    multiplier *= 2.0; // Fixed 2x multiplier
                if (num1.upgrades.ContainsKey("BottomRow5x") && num1.upgrades["BottomRow5x"].level > 1)
                    multiplier *= 5.0; // Fixed 5x multiplier (stacks with 2x)
            }
        }
        
        return multiplier;
    }
    
    // Get global multiplier from Numbers 4-6
    public double GetGlobalMultiplier()
    {
        double multiplier = 1.0;
        
        for (int i = 4; i <= 6; i++)
        {
            if (allNumbers.ContainsKey(i) && allNumbers[i].isUnlocked)
            {
                var num = allNumbers[i];
                string key = $"GlobalMultiplier{i}";
                if (num.upgrades.ContainsKey(key) && num.upgrades[key].level > 1)
                {
                    multiplier *= num.upgrades[key].effect;
                }
            }
        }
        
        return multiplier;
    }
    
    // Get automation speed upgrade cost
    public (int number, double amount) GetAutomationSpeedCost()
    {
        // Pattern: 1 4, 2 4s, 5 4s, 1 5, 2 5s, 5 5s, 1 6, etc.
        int level = automationSpeedLevel;
        int[] amounts = { 1, 2, 5 };
        int baseNumber = 4;
        
        int numberIndex = level / 3;
        int amountIndex = level % 3;
        
        int targetNumber = baseNumber + numberIndex;
        double targetAmount = amounts[amountIndex];
        
        return (targetNumber, targetAmount);
    }
    
    // Purchase automation speed upgrade
    public bool PurchaseAutomationSpeedUpgrade()
    {
        var (number, amount) = GetAutomationSpeedCost();
        
        if (allNumbers.ContainsKey(number) && allNumbers[number].amount >= amount)
        {
            allNumbers[number].amount -= amount;
            automationSpeedLevel++;
            
            // Halve the automation speed (minimum 0.5 seconds)
            automationSpeed = Mathf.Max(0.5f, (float)(automationSpeed / 2.0));
            
            // Round to 0.5 if below 1 second
            if (automationSpeed < 1.0f && automationSpeed > 0.5f)
            {
                automationSpeed = 0.5f;
            }
            
            return true;
        }
        return false;
    }
    
    public bool HasAutomationUpgrade(int number)
    {
        if (number == 2 && allNumbers.ContainsKey(2))
        {
            return allNumbers[2].upgrades.ContainsKey("FirstHalfAutomation") && 
                   allNumbers[2].upgrades["FirstHalfAutomation"].level > 1;
        }
        else if (number == 3 && allNumbers.ContainsKey(3))
        {
            return allNumbers[3].upgrades.ContainsKey("SecondHalfAutomation") && 
                   allNumbers[3].upgrades["SecondHalfAutomation"].level > 1;
        }
        return false;
    }
    
    // Reset all Number data (called on hard reset)
    public void ResetAllNumbers()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (allNumbers.ContainsKey(i))
            {
                allNumbers[i].amount = 0;
                allNumbers[i].isUnlocked = false;
                allNumbers[i].InitializeDefaultUpgrades();
            }
        }
        
        automationSpeed = 30.0;
        automationSpeedLevel = 0;
    }
}

